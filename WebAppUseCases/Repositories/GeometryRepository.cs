using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;

namespace WebAppUseCases.Repositories;

public class GeometryRepository : IGeometryRepository<Geometry>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
        string sql = "CREATE TYPE fragment_original_id AS (fragment POLYGON, original_id INT)";
        geometryDbContext.Database.ExecuteSqlRaw(sql);
    }
    
    public async Task<Geometry> GetGeometryByPolygon(Polygon polygon)
    {
        var res = await _geometryDbContext.GeometryOriginals
            .Where(g => g.Data.Intersects(polygon))
            .Select(g => g.Data.Intersection(polygon))
            .ToArrayAsync();
        return new GeometryCollection(res);
    }
    
    public async Task<Geometry> GetGeometryByPolygonFast(Polygon polygon)
    {
        string sql = @"
            DO $$ 
            DECLARE 
                original_ids INT[] := ARRAY[]::INT[];
                fragments fragment_original_id[];
                originals GEOMETRY[] := ARRAY[]::GEOMETRY[];
            BEGIN
                SELECT ARRAY
                (
                    SELECT ROW(""Fragment"", ""GeometryOriginalId"")::fragment_original_id FROM ""GeometryFragments"" AS f WHERE f.""Fragment"" && {0}
                ) 
                INTO fragments;    

                FOR fragment IN ARRAY(fragments) LOOP
                    IF fragment.fragment_original_id <> ANY(original_ids) THEN
                        IF ST_Intersects(fragment.fragment, {0}) THEN
                                original_ids := array_append(original_ids, fragment.fragment_original_id);
                        END IF;
                    END IF;
                END LOOP;

                SELECT ST_Intersection(f.""Data"", {0}) INTO originals FROM ""GeometryOriginals"" AS f WHERE f.""GeometryOriginalId"" = ANY(original_ids);

                -- Возвращаем массив
                RETURN QUERY SELECT originals;
            END $$;";
        var result = await _geometryDbContext.Database.SqlQueryRaw<Geometry>(sql, polygon).ToArrayAsync();
        
        return new GeometryCollection(result);
    }
    
    public Task<Geometry> GetSimplifiedGeometryByPolygon(Polygon polygon, double tolerance)
    {
        return _geometryDbContext.Database.SqlQueryRaw<Geometry>(
            @"
                SELECT 
                    ST_INTERSECTION(
                        ST_SimplifyPreserveTopology(
                            ST_Collect(
                                (SELECT f.""Data"" FROM ""GeometryOriginals"" AS f WHERE ST_Intersects(f.""Data"", {0}))
                            ), 
                            {1}
                        ), 
                        {0}
                    ) AS ""Value""
                ", polygon, tolerance).FirstOrDefaultAsync()!;
    }
}