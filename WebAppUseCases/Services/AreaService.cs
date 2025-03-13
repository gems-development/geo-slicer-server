using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;
using WebAppUseCases.Services.Interfaces;

namespace WebAppUseCases.Services;

public class AreaService<TGeometry> : IAreaService<TGeometry> where TGeometry : Geometry
{
    private IGeometryRepository<TGeometry> _geometryRepository;

    public AreaService(IGeometryRepository<TGeometry> geometryRepository)
    {
        _geometryRepository = geometryRepository;
    }

    public async Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop)
    {
        return await _geometryRepository.GetGeometryByRectangle(pointLeftBottom, pointRightTop);
    }
}