using NetTopologySuite.Geometries;

namespace Utils;

public static class PolygonUtils
{
    public static IEnumerable<LineString> GetAllLines(Polygon polygon)
    {
        foreach (LineString lineString in LineStringUtils.Split(polygon.Shell))
        {
            yield return lineString;
        }

        foreach (LinearRing hole in polygon.Holes)
        {
            foreach (LineString lineString in LineStringUtils.Split(hole))
            {
                yield return lineString;
            }
        }
    }
    
    public static ISet<LineString> GetAllLinesSet(Polygon polygon)
    {
        ISet<LineString> linesSet = new HashSet<LineString>();
        foreach (LineString lineString in LineStringUtils.Split(polygon.Shell))
        {
            linesSet.Add(lineString);
        }

        foreach (LinearRing hole in polygon.Holes)
        {
            foreach (LineString lineString in LineStringUtils.Split(hole))
            {
                linesSet.Add(lineString);
            }
        }

        return linesSet;
    }
    
    public static Polygon FromRectangle(Point pointLeftBottom, Point pointRightTop, int srid)
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
        var polygon = new Polygon(ring);
        polygon.SRID = srid;
        return polygon;
    }
}