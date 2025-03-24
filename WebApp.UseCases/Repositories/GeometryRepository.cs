using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;

namespace WebApp.UseCases.Repositories;

public class GeometryRepository : IGeometryRepository<Geometry>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<Geometry> GetGeometryByPolygonLinq(Polygon polygon)
    {
        var res = await _geometryDbContext.GeometryOriginals
            .Where(g => g.Data.Intersects(polygon))
            .Select(g => g.Data.Intersection(polygon))
            .ToArrayAsync();
        return new GeometryCollection(res);
    }

    public Task<Geometry> GetGeometryByPolygonEnumerateFragments(Polygon polygon)
    {
        string sql = @"SELECT get_intersecting_geometry_by_search_fragments({0}) AS ""Value""";
        
        return _geometryDbContext.Database
            .SqlQueryRaw<Geometry>(sql, polygon)
            .FirstAsync();
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