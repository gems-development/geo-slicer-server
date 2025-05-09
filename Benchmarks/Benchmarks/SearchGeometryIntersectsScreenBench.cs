using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Benchmarks.Benchmarks;

//бенчмарк не актуален
//todo добавить GeometryServerSrid
public class SearchGeometryIntersectsScreenBench
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
    
    private static readonly Polygon VeryBigScreen = new Polygon(new LinearRing(new Coordinate[]
    {
        new Coordinate(95.816, 45.205),
        new Coordinate(117.343, 45.205),
        new Coordinate(117.343, 63.462),
        new Coordinate(95.816, 63.462),
        new Coordinate(95.816, 45.205)
    }));

    private static readonly PostgreApplicationContext PgContext =
        new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");
    
    private string _whereExistsSql = @"
        WITH
	        fragments_in_bounding_box AS 
	        (
		        select ""GeometryOriginalId"" as original_id, ""Fragment"" as fragment from ""GeometryFragments""
		        where ""Fragment"" && {0}),
		        
	        original_geometry_id AS (
		        select original_id as id from (select distinct original_id from fragments_in_bounding_box) as ext_table
		        where exists(
			        select f.original_id from fragments_in_bounding_box as f 
				        where ext_table.original_id = f.original_id AND (f.fragment @ {0} OR ST_Intersects(
					        f.fragment, {0}))))
					        
            SELECT * FROM original_geometry_id
        ";
    private string _enumerateSql = "SELECT * FROM  get_intersecting_geometry_by_search_fragments({0})";

    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsOnSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_enumerateSql, ScreenSmall)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsWithWhereExistsOnSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_whereExistsSql, ScreenSmall)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonLinqOnSmallScreen()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ScreenSmall)).Select(g => g.Id)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsOnBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_enumerateSql, ScreenBig)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsWithWhereExistsOnBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_whereExistsSql, ScreenBig)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonLinqOnBigScreen()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ScreenBig)).Select(g => g.Id)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsOnFullScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_enumerateSql, ScreenFull)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsWithWhereExistsOnFullScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_whereExistsSql, ScreenFull)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonLinqOnFullScreen()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ScreenFull)).Select(g => g.Id)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsOnVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_enumerateSql, VeryBigScreen)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonEnumerateFragmentsWithWhereExistsOnVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<int>(_whereExistsSql, VeryBigScreen)
            .ToArray();
    }
    
    [Benchmark]
    public void GetGeometryByPolygonLinqOnVeryBigScreen()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(VeryBigScreen)).Select(g => g.Id)
            .ToArray();
    }
}