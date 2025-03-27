using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Repositories;

public class GeometryRepository : IGeometryRepository<Geometry>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByLinearRing(LinearRing ring, CancellationToken cancellationToken)
    {
        var res = await _geometryDbContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ring))
            .Select(g => g.Data.Intersection(ring))
            .Select(g => new AreaIntersectionDto<Geometry>("NAME", g))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return res;
    }
}