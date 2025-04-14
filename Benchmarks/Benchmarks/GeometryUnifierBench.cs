using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
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
        PgContext.Database
            .SqlQueryRaw<Geometry>(
                "SELECT ST_Union(f.\"Fragment\") AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", {0})",
                ScreenSmall).FirstOrDefault();
    }
    
    [Benchmark]
    public void TestGeometryUnifierSmallScreenNtsUnion()
    {
        UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => ScreenSmall.Intersects(o.Fragment)).Select(o => o.Fragment));
    }

    [Benchmark]
    public void TestGeometryIntersectionSmallScreen()
    {
        PgContext.GeometryOriginals.Select(o => o.Data.Intersection(ScreenSmall)).First();
    }

    [Benchmark]
    public void TestGeometryUnifierBigScreen()
    {
        PgContext.Database
            .SqlQueryRaw<Geometry>(
                "SELECT ST_Union(f.\"Fragment\") AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", {0})",
                ScreenBig).FirstOrDefault();
    }
    
    [Benchmark]
    public void TestGeometryUnifierBigScreenNtsUnion()
    {
        UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => ScreenBig.Intersects(o.Fragment)).Select(o => o.Fragment));
    }

    [Benchmark]
    public void TestGeometryIntersectionBigScreen()
    {
        PgContext.GeometryOriginals.Select(o => o.Data.Intersection(ScreenBig)).First();
    }

    [Benchmark]
    public void TestGeometryUnifierFullScreen()
    {
        PgContext.Database
            .SqlQueryRaw<Geometry>(
                "SELECT ST_Union(f.\"Fragment\") AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", {0})",
                ScreenFull).FirstOrDefault();
    }
    
    [Benchmark]
    public void TestGeometryUnifierFullScreenNtsUnion()
    {
        UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => ScreenFull.Intersects(o.Fragment)).Select(o => o.Fragment));
    }

    [Benchmark]
    public void TestGeometryIntersectionFullScreen()
    {
        PgContext.GeometryOriginals.Select(o => o.Data.Intersection(ScreenFull)).First();
    }
}