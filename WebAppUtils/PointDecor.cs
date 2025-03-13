using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace WebAppUtils;

public class PointDecor : Point
{
    [JsonConstructor]
    public PointDecor(double x, double y) : base(x, y)
    {
    }
}