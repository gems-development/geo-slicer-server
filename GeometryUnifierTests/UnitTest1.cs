using DataAccess.PostgreSql;
using DataAccess.Repositories.GeometryUnion;
using GeoSlicer.Utils;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Union;
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

    [Fact]
    public void TestGeometryUnifierSmallScreen()
    {
        var nts = UnaryUnionOp.Union(PgContext.GeometryFragments
            .Where(o => ScreenSmall.Intersects(o.Fragment)).Select(o => o.Fragment));;

        var data = PgContext.Database
            .SqlQueryRaw<Geometry>("SELECT ST_Union(f.\"Fragment\") AS \"Value\" FROM \"GeometryFragments\" AS f WHERE ST_Intersects(f.\"Fragment\", {0})", ScreenSmall).FirstOrDefault();
        
        WriteGeometryToFile("TestGeometryUnifierSmallScreen\\screen.geojson",ScreenSmall);
        WriteGeometryToFile("TestGeometryUnifierSmallScreen\\nts.geojson", nts);
        WriteGeometryToFile("TestGeometryUnifierSmallScreen\\data.geojson", data);

        var frames = PgContext.GeometryFragments
            .Where(o => ScreenSmall.Intersects(o.Fragment)).Select(o => o.Fragment).ToArray();

        for (int i = 0; i < frames.Length; i++)
        {
            WriteGeometryToFile($"TestGeometryUnifierSmallScreen\\data{i}.geojson", frames[i]);
        }
        
        //var original = PgContext.Database
        //    .SqlQueryRaw<Geometry>("SELECT ST_Union(f.\"Fragment\") AS \"Value\" FROM \"GeometryFragments\" AS f").FirstOrDefault();
        
        var original = UnaryUnionOp.Union(PgContext.GeometryFragments
            .Select(o => o.Fragment));;
        
        WriteGeometryToFile("TestGeometryUnifierSmallScreen\\original.geojson", original);
    }
    
    private static void WriteGeometryToFile<T>(string path, T geometry) where T : Geometry
    {
        var geoJson = new GeoJsonWriter().Write(geometry);
        File.WriteAllText(path, geoJson);
    }
}