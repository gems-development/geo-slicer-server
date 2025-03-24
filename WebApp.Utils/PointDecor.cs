using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace WebApp.Utils;

public class PointDecor : Point
{
    [JsonConstructor]
    public PointDecor(double x, double y) : base(x, y)
    {
    }
}