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
    }
    
    public async Task<Geometry> GetGeometryByLinearRing(LinearRing ring, CancellationToken cancellationToken)
    {
        var res = await _geometryDbContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ring))
            .Select(g => g.Data.Intersection(ring))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return new GeometryCollection(res);
    }
}