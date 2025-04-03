using NetTopologySuite.Geometries;
using WebApp.Services.Interfaces;

namespace WebApp.Services;

public class RectangleToPolygonService : IRectangleToPolygonService
{
    public Polygon CreatePolygon(Point pointLeftBottom, Point pointRightTop)
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
        return new Polygon(ring);
    }
}