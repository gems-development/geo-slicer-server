using System.Globalization;
using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Benchmarks.Benchmarks;

[MemoryDiagnoser(false)]
public class GeometryOuterClickBench
{
    private static readonly Coordinate outerCoordinate = new (108.7126551, 53.8585095);

    private static readonly PostgreApplicationContext PgContext =
        new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");

    [Benchmark]
    public void TestGeometryOuterClickWithFragments()
    {
        var x = outerCoordinate.X.ToString(CultureInfo.InvariantCulture);
        var y = outerCoordinate.Y.ToString(CultureInfo.InvariantCulture);
        var result = PgContext.Database
            .SqlQueryRaw<long>(
                $"SELECT f.\"GeometryOriginalId\" AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", ST_GeomFromText('POINT({x} {y})'))")
            .ToArray();
    }

    [Benchmark]
    public void TestGeometryOuterClickWithOriginal()
    {
        var x = outerCoordinate.X.ToString(CultureInfo.InvariantCulture);
        var y = outerCoordinate.Y.ToString(CultureInfo.InvariantCulture);
        var result = PgContext.Database
            .SqlQueryRaw<long>(
                $"SELECT f.\"Id\" AS \"Value\" FROM \"GeometryOriginals\" AS f WHERE ST_Intersects(f.\"Data\", ST_GeomFromText('POINT({x} {y})'))")
            .ToArray();
    }
}