using NetTopologySuite.Geometries;

namespace Utils;

public static class LineStringUtils
{
    public static IEnumerable<LineString> Split(LineString sequence)
    {
        for (int i = 0; i < sequence.Count - 1; i++)
        {
            yield return new LineString(new[] { sequence[i], sequence[i + 1] });
        }
    }
}