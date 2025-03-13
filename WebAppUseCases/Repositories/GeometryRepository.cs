using DataAccess.Interfaces;
using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;

namespace WebAppUseCases.Repositories;

public class GeometryRepository : IGeometryRepository<MultiPolygon>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<MultiPolygon> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop)
    {
        throw new NotImplementedException();
    }
}