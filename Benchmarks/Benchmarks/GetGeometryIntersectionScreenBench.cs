using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Benchmarks.Benchmarks;

//бенчмарк не актуален
//todo добавить GeometryServerSrid
public class GetGeometryIntersectionScreenBench
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
    
    private static readonly Polygon VeryBigScreenIntersectsLeftBaikals = new GeometryFactory().CreatePolygon(new[]
    {
        new Coordinate(99.4985, 46.0831),
        new Coordinate(new Coordinate(115.178, 61.333).X, new Coordinate(99.4985, 46.0831).Y),
        new Coordinate(115.178, 61.333),
        new Coordinate(new Coordinate(99.4985, 46.0831).X, new Coordinate(115.178, 61.333).Y), 
        new Coordinate(99.4985, 46.0831) 
    });

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
					        
            SELECT ST_Intersection(""Data"", {0})  FROM ""GeometryOriginals""
            WHERE ""Id"" = ANY(SELECT id FROM original_geometry_id)
        ";
    private string _enumerateSql = @"
        SELECT ST_Intersection(""Data"", {0})  FROM ""GeometryOriginals""
        WHERE ""Id"" = ANY(SELECT * from get_intersecting_geometry_by_search_fragments({0}))";

    private string _enumerateBbSql = @"
           SELECT ""Data"" AS result  FROM ""GeometryOriginals""
           WHERE ""Data"" @ {0}
           UNION
           SELECT ST_Intersection(""Data"", {0}) AS result  FROM ""GeometryOriginals""
           WHERE ""Id"" = ANY(SELECT * from get_geometry_ids_intersects_display_bb_borders({0}))
           ";

    private string _geometryOriginalCollectionDisplayIntersection = @"
            SELECT ""Data"" FROM get_geometry_original_collection_display_intersection({0})
            ";

    private string _geometryOriginalCollectionDisplayIntersectionSql = @"
    WITH 	
	    originals_in_display AS MATERIALIZED (SELECT * FROM ""GeometryOriginals"" AS f WHERE f.""Data"" @ {0}),
	    fragments_in_display_ids AS MATERIALIZED  (
		    SELECT DISTINCT f.""GeometryOriginalId"" AS id
            FROM ""GeometryFragments"" AS f 
            WHERE f.""Fragment"" @ {0} AND NOT (f.""GeometryOriginalId"" = ANY(SELECT ""Id"" FROM originals_in_display))),
        fragments_intersects_display_ids AS NOT MATERIALIZED (
		    SELECT DISTINCT f.""GeometryOriginalId"" AS id
		        FROM ""GeometryFragments"" AS f 
		        WHERE 
		    	    (f.""Fragment"" && {0} AND NOT f.""Fragment"" @ {0})
		    	    AND NOT (f.""GeometryOriginalId"" = ANY(SELECT id FROM fragments_in_display_ids))
		    	    AND ST_INTERSECTS(f.""Fragment"", {0}))			    	
    SELECT ""Data"" FROM originals_in_display
    UNION ALL
    SELECT ST_Intersection(f.""Data"", {0}) AS ""Data""
	    FROM ""GeometryOriginals"" AS f
	    WHERE f.""Id"" = ANY(SELECT id FROM fragments_in_display_ids)
    UNION ALL
    SELECT ST_Intersection(f.""Data"", {0}) AS ""Data""
	    FROM ""GeometryOriginals"" AS f
	    WHERE f.""Id"" = ANY(SELECT id FROM fragments_intersects_display_ids)
    ";
    
    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionSqlSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersectionSql, ScreenSmall)
            .ToArray();
    }

    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersection, ScreenSmall)
            .ToArray();
    }

    [Benchmark]
    public void EnumerateBbSqlSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateBbSql, ScreenSmall)
            .ToArray();
    }
    
    [Benchmark]
    public void EnumerateSqlSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateSql, ScreenSmall)
            .ToArray();
    }
    
    [Benchmark]
    public void WhereExistsSqlSmallScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_whereExistsSql, ScreenSmall)
            .ToArray();
    }
    
    
    [Benchmark]
    public void LinqOnSmallScreen()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ScreenSmall)).Select(g => g.Data.Intersection(ScreenSmall))
            .ToArray();
    }
    
    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionSqlScreenBig()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersectionSql, ScreenBig)
            .ToArray();
    }

    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionScreenBig()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersection, ScreenBig)
            .ToArray();
    }

    [Benchmark]
    public void EnumerateBbSqlScreenBig()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateBbSql, ScreenBig)
            .ToArray();
    }
    
    [Benchmark]
    public void EnumerateSqlScreenBig()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateSql, ScreenBig)
            .ToArray();
    }
    
    [Benchmark]
    public void WhereExistsSqlScreenBig()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_whereExistsSql, ScreenBig)
            .ToArray();
    }
    
    
    [Benchmark]
    public void LinqOnScreenBig()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ScreenBig)).Select(g => g.Data.Intersection(ScreenBig))
            .ToArray();
    }
    
    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionSqlScreenFull()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersectionSql, ScreenFull)
            .ToArray();
    }

    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionScreenFull()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersection, ScreenFull)
            .ToArray();
    }

    [Benchmark]
    public void EnumerateBbSqlScreenFull()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateBbSql, ScreenFull)
            .ToArray();
    }
    
    [Benchmark]
    public void EnumerateSqlScreenFull()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateSql, ScreenFull)
            .ToArray();
    }
    
    [Benchmark]
    public void WhereExistsSqlScreenFull()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_whereExistsSql, ScreenFull)
            .ToArray();
    }
    
    
    [Benchmark]
    public void LinqOnScreenFull()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(ScreenFull)).Select(g => g.Data.Intersection(ScreenFull))
            .ToArray();
    }
    
    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionSqlVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersectionSql, VeryBigScreen)
            .ToArray();
    }

    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersection, VeryBigScreen)
            .ToArray();
    }

    [Benchmark]
    public void EnumerateBbSqlVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateBbSql, VeryBigScreen)
            .ToArray();
    }
    
    [Benchmark]
    public void EnumerateSqlVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateSql, VeryBigScreen)
            .ToArray();
    }
    
    [Benchmark]
    public void WhereExistsSqlVeryBigScreen()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_whereExistsSql, VeryBigScreen)
            .ToArray();
    }
    
    
    [Benchmark]
    public void LinqOnVeryBigScreen()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(VeryBigScreen)).Select(g => g.Data.Intersection(VeryBigScreen))
            .ToArray();
    }
    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionSqlVeryBigScreenIntersectsLeftBaikals()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersectionSql, VeryBigScreenIntersectsLeftBaikals)
            .ToArray();
    }

    [Benchmark]
    public void GeometryOriginalCollectionDisplayIntersectionVeryBigScreenIntersectsLeftBaikals()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_geometryOriginalCollectionDisplayIntersection, VeryBigScreenIntersectsLeftBaikals)
            .ToArray();
    }

    [Benchmark]
    public void EnumerateBbSqlVeryBigScreenIntersectsLeftBaikals()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateBbSql, VeryBigScreenIntersectsLeftBaikals)
            .ToArray();
    }
    
    [Benchmark]
    public void EnumerateSqlVeryBigScreenIntersectsLeftBaikals()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_enumerateSql, VeryBigScreenIntersectsLeftBaikals)
            .ToArray();
    }
    
    [Benchmark]
    public void WhereExistsSqlVeryBigScreenIntersectsLeftBaikals()
    {
        var res = PgContext.Database
            .SqlQueryRaw<Geometry>(_whereExistsSql, VeryBigScreenIntersectsLeftBaikals)
            .ToArray();
    }
    
    
    [Benchmark]
    public void LinqOnVeryBigScreenIntersectsLeftBaikals()
    {
        var res = PgContext.GeometryOriginals
            .Where(g => g.Data.Intersects(VeryBigScreenIntersectsLeftBaikals)).Select(g => g.Data.Intersection(VeryBigScreenIntersectsLeftBaikals))
            .ToArray();
    }
}