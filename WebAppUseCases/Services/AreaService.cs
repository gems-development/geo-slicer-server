using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;
using WebAppUseCases.Services.Interfaces;

namespace WebAppUseCases.Services;

public class AreaService<TGeometry> : IAreaService<TGeometry> where TGeometry : Geometry
{
    private readonly IGeometryRepository<TGeometry> _geometryRepository;

    public AreaService(IGeometryRepository<TGeometry> geometryRepository)
    {
        _geometryRepository = geometryRepository;
    }

    public Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken)
    {
        Coordinate coordinateLeftTop = new Coordinate(pointLeftBottom.X, pointRightTop.Y);
        Coordinate coordinateRightBottom = new Coordinate(pointRightTop.X, pointLeftBottom.Y);
        LinearRing ring = new LinearRing(new []
        {
            pointLeftBottom.Coordinate,
            coordinateLeftTop,
            pointRightTop.Coordinate,
            coordinateRightBottom, 
            pointLeftBottom.Coordinate
        });
        
        return _geometryRepository.GetGeometryByLinearRing(ring, cancellationToken);
    }
}