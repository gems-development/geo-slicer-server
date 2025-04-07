using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Services.GeometryProviders.Interfaces;

namespace Services.GeometryProviders;

public class GeometryByScreenService : IGeometryByScreenService<Geometry>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryByScreenService(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(Polygon screen, CancellationToken cancellationToken)
    {
        var res = await _geometryDbContext.GeometryOriginals
            .Where(g => g.Data.Intersects(screen))
            .Select(g => 
                new AreaIntersectionDto<Geometry>(
                    g.Layer.Alias, g.Properties, g.Data.Intersection(screen)))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return res;
    }

    private Task<Geometry> GetSimplifiedGeometryByPolygon(Polygon polygon, double tolerance)
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