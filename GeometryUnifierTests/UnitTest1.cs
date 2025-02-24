using DataAccess.PostgreSql;
using DataAccess.Repositories.GeometryUnion;
using GeoSlicer.Utils;
using NetTopologySuite.Geometries;
using Xunit.Abstractions;

namespace GeometryUnifierTests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;
    
    private static readonly Polygon Screen = new Polygon(new LinearRing(new Coordinate[]
        { new(106.92672, 53.48174), new(107.74302, 53.47658), new(107.74007, 52.87246), new(106.92745, 52.86951), new(106.92672, 53.48174) }));

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test()
    {
        String connectionString = "Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin";
        var pgContext = new PostgreApplicationContext(connectionString);
        var polygons = pgContext.GeometryFragments
            .Where(o => o.GeometryOriginalId == 21)
            .Select(o => o.Fragment);
        var result = GeometryUnifier.Union(polygons);
    }

    [Fact]
    public void TestWriteFile()
    {
        String connectionString = "Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin";
        var pgContext = new PostgreApplicationContext(connectionString); 
        var multiPolygon = new MultiPolygon(pgContext.GeometryFragments.Where(o => o.GeometryOriginalId == 21).Select(o => o.Fragment).ToArray());
        GeoJsonFileService.WriteGeometryToFile(multiPolygon, "C:\\Users\\Даниил\\Desktop\\geo-slicer-server\\Benchmarks\\slicedBaikal1500.geojson.ignore", true);
    }
}