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
}