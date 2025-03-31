using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace WebApp.Utils.Dto.Requests;

public class PointDto
{
    public double X { get; set; }
    public double Y { get; set; }

    [JsonConstructor]
    public PointDto(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Point CreatePoint()
    {
        return new Point(X, Y);
    }
}