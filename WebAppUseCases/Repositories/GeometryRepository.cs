using DataAccess.Interfaces;
using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;

namespace WebAppUseCases.Repositories;

public class GeometryRepository<TGeometry> : IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    private GeometryDbContext _geometryDbContext;

    public GeometryRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop)
    {
        throw new NotImplementedException();
    }
}