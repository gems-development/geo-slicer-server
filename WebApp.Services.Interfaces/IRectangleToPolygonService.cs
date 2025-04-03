using NetTopologySuite.Geometries;

namespace WebApp.Services.Interfaces;

public interface IRectangleToPolygonService
{ 
    Polygon CreatePolygon(Point pointLeftBottom, Point pointRightTop);
}