using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Interfaces;

namespace WebApp.UseCases;

public class AreaUseCase<TGeometry> : IAreaUseCase<TGeometry> where TGeometry : Geometry
{
    private readonly IGeometryRepository<TGeometry> _geometryRepository;

    public AreaUseCase(IGeometryRepository<TGeometry> geometryRepository)
    {
        _geometryRepository = geometryRepository;
    }

    public Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken)
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
        
        Polygon polygon = new Polygon(ring);
        
        double rectangleLength = pointRightTop.X - pointLeftBottom.X;
        
        return _geometryRepository.GetGeometryByPolygonLinq(polygon, cancellationToken);
    }
}