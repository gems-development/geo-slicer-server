using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Services.Interfaces;

namespace WebApp.UseCases.Services;

public class AreaService<TGeometry> : IAreaService<TGeometry> where TGeometry : Geometry
{
    private IGeometryRepository<TGeometry> _geometryRepository;

    public AreaService(IGeometryRepository<TGeometry> geometryRepository)
    {
        _geometryRepository = geometryRepository;
    }

    public Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop)
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
        
        return _geometryRepository.GetGeometryByPolygonLinq(polygon);
    }
}