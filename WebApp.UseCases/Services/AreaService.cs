using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Services.Interfaces;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Services;

public class AreaService<TGeometry> : IAreaService<TGeometry> where TGeometry : Geometry
{
    private readonly IGeometryRepository<TGeometry> _geometryRepository;

    public AreaService(IGeometryRepository<TGeometry> geometryRepository)
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