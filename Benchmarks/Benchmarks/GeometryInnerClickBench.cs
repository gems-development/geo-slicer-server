using System.Globalization;
using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Utils;

namespace Benchmarks.Benchmarks;
//todo актуальный бенчмарк

//бенчмарк должен проводится на бд, в которой сохранено 145 фигур из папки TestFiles

//бенчмарк проводился на фигурах, фрагменты которых имеют не более 1000 точек

[MemoryDiagnoser(false)]
public class GeometryInnerClickBench
{
    private readonly LineString innerRing;
    
    private static readonly Coordinate innerCoordinate = new Coordinate(106.1756565, 51.9990953);
    
    private static readonly PostgreApplicationContext PgContext =
        new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");

    public GeometryInnerClickBench()
    {
        innerRing = new (new[]
        {
            new Coordinate(106.1756565, 51.9990953),
            new Coordinate(106.1766145, 51.9990098),
            new Coordinate(106.1764491, 51.9984510)
        });
        innerRing.SRID = GeometryServerSrid.Srid;
    }

    [Benchmark]
    public void TestGeometryInnerClickWithFragments()
    {
        var x = innerCoordinate.X.ToString(CultureInfo.InvariantCulture);
        var y = innerCoordinate.Y.ToString(CultureInfo.InvariantCulture);
        var result = PgContext.Database
            .SqlQueryRaw<long>(
                $"SELECT f.\"GeometryOriginalId\" AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", ST_GeomFromText('POINT({x} {y})', {GeometryServerSrid.Srid}))").ToArray();
    }
    
    [Benchmark]
    public void TestGeometryInnerClickWithOriginal()
    {
        var x = innerCoordinate.X.ToString(CultureInfo.InvariantCulture);
        var y = innerCoordinate.Y.ToString(CultureInfo.InvariantCulture);
        var result = PgContext.Database
            .SqlQueryRaw<long>(
                $"SELECT f.\"Id\" AS \"Value\" FROM \"GeometryOriginals\" AS f WHERE ST_Intersects(f.\"Data\", ST_GeomFromText('POINT({x} {y})', {GeometryServerSrid.Srid}))").ToArray();
    }
    
    [Benchmark]
    public void TestGeometryInnerLinerRingIntersectsWithFragments()
    {
        var result = PgContext.Database
            .SqlQueryRaw<long>(
                "SELECT f.\"GeometryOriginalId\" AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", {0})", innerRing).ToArray();
    }
    
    [Benchmark]
    public void TestGeometryInnerLinerRingIntersectsWithOriginal()
    {
        var result = PgContext.Database
            .SqlQueryRaw<long>(
                "SELECT f.\"Id\" AS \"Value\" FROM \"GeometryOriginals\" AS f WHERE ST_Intersects(f.\"Data\", {0})", innerRing).ToArray();
    }
    
    
    
    [Benchmark]
    public void TestGeometryInnerLinerRingIntersectsWithFragmentsLinq()
    {
        var result = PgContext.GeometryFragments.Where(g => g.Fragment.Intersects(innerRing)).Select(g => g.GeometryOriginalId).ToArray();
    }
    
    [Benchmark]
    public void TestGeometryInnerLinerRingIntersectsWithOriginalLinq()
    {
        var result = PgContext.GeometryOriginals.Where(g => g.Data.Intersects(innerRing)).Select(g => g.Id).ToArray();
    }
}