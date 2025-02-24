using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;

namespace Benchmarks.Benchmarks;

[MemoryDiagnoser(false)]
public class GeometryUnifierBench
{
    private static readonly Polygon ScreenSmall = new Polygon(new LinearRing(new Coordinate[]
    {
        new(106.92672, 53.48174), new(107.74302, 53.47658), new(107.74007, 52.87246), new(106.92745, 52.86951),
        new(106.92672, 53.48174)
    }));

    private static readonly Polygon ScreenFull = new Polygon(new LinearRing(new Coordinate[]
        { new(103, 57), new(111, 57), new(111, 51), new(103, 51), new(103, 57) }));

    private static readonly Polygon ScreenBig = new Polygon(new LinearRing(new Coordinate[]
    {
        new(104.3503, 51.6423), new(104.6685, 55.0666), new(109.4779, 55.2317), new(109.3306, 51.4360),
        new(104.3503, 51.6423)
    }));

    private static readonly PostgreApplicationContext PgContext =
        new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");

    [Benchmark]
    public void TestGeometryUnifierSmallScreen()
    {
        UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => o.GeometryOriginalId == 22 && ScreenSmall.Intersects(o.Fragment)).Select(o => o.Fragment));
    }

    [Benchmark]
    public void TestGeometryIntersectionSmallScreen()
    {
        PgContext.GeometryOriginals.Where(o => o.Id == 22).Select(o => o.Data).First().Intersection(ScreenSmall);
    }

    [Benchmark]
    public void TestGeometryUnifierBigScreen()
    {
        UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => o.GeometryOriginalId == 22 && ScreenBig.Intersects(o.Fragment)).Select(o => o.Fragment));
    }

    [Benchmark]
    public void TestGeometryIntersectionBigScreen()
    {
        PgContext.GeometryOriginals.Where(o => o.Id == 22).Select(o => o.Data).First().Intersection(ScreenBig);
    }

    [Benchmark]
    public void TestGeometryUnifierFullScreen()
    {
        UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => o.GeometryOriginalId == 22 && ScreenFull.Intersects(o.Fragment)).Select(o => o.Fragment));
    }

    [Benchmark]
    public void TestGeometryIntersectionFullScreen()
    {
        PgContext.GeometryOriginals.Where(o => o.Id == 22).Select(o => o.Data).First().Intersection(ScreenFull);
    }
}