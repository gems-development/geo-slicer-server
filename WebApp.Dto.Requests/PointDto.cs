using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using Utils;

namespace WebApp.Dto.Requests;

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
        var point = new Point(X, Y);
        point.SRID = GeometryServerSrid.Srid;
        return point;
    }
}