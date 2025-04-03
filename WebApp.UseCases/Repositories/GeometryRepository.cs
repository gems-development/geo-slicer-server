using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using WebApp.UseCases.Repositories.Interfaces;

namespace WebApp.UseCases.Repositories;

public class GeometryRepository : IGeometryRepository<Geometry>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByPolygonLinq(Polygon polygon, CancellationToken cancellationToken)
    {
        var res = await _geometryDbContext.GeometryOriginals
            .Where(g => g.Data.Intersects(polygon))
            .Select(g => g.Data.Intersection(polygon))
            .Select(g => new AreaIntersectionDto<Geometry>("water", "BAIKAL", g))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return res;
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