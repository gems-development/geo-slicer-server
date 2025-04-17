using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Benchmarks.Benchmarks;

public class GetGeometryIntersectionNewBench
{
	private static readonly PostgreApplicationContext PgContext =
		new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");
    private Polygon FullScreen { get; set; }
    
    private Polygon ScreenIntersectsThreeLeftBaikals { get; set; }
    
    private Polygon ScreenIntersectsTwelveBaikals { get; set; }
    
    private Polygon ScreenIntersectsFiveTopBaikals { get; set; }
    
    private Polygon ScreenIntersectsFiveTopAndFiveBottomBaikals { get; set; }
    
    public GetGeometryIntersectionNewBench()
    {
        Coordinate leftBottom = new Coordinate(96.480, 44.269);
        Coordinate rightTop = new Coordinate(116.940, 62.598); 
        
        FullScreen = CreateRectanglePolygon(leftBottom, rightTop);
        
        leftBottom = new Coordinate(100.051, 45.041);
        rightTop = new Coordinate(117.107, 62.670); 
        
        ScreenIntersectsThreeLeftBaikals = CreateRectanglePolygon(leftBottom, rightTop);
        
        leftBottom = new Coordinate(99.4070, 46.4849);
        rightTop = new Coordinate(114.1381, 60.5477); 
        
        ScreenIntersectsTwelveBaikals = CreateRectanglePolygon(leftBottom, rightTop);
        
        leftBottom = new Coordinate(96.480, 44.269);
        rightTop = new Coordinate(116.046, 56.63072);
        
        ScreenIntersectsFiveTopBaikals = CreateRectanglePolygon(leftBottom, rightTop);
        
        leftBottom = new Coordinate(96.480, 50.56485);
        rightTop = new Coordinate(116.046, 56.63072);
        
        ScreenIntersectsFiveTopAndFiveBottomBaikals = CreateRectanglePolygon(leftBottom, rightTop);
    }

    private static Polygon CreateRectanglePolygon(Coordinate leftBottom, Coordinate rightTop)
    {
        return new GeometryFactory().CreatePolygon(new[]
        {
            new Coordinate(leftBottom.X, leftBottom.Y),
            new Coordinate(rightTop.X, leftBottom.Y),
            new Coordinate(rightTop.X, rightTop.Y),    
            new Coordinate(leftBottom.X, rightTop.Y),   
            new Coordinate(leftBottom.X, leftBottom.Y) 
        });
    }
    
    private string _enumerateFragmentsSql = @"
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

    private string _enumerateOriginalsSql = @"
		SELECT ""Data""
		FROM ""GeometryOriginals"" AS f
		WHERE f.""Data"" @ {0}
		UNION ALL 
		SELECT ST_Intersection(""Data"", {0}) AS ""Data""
		FROM ""GeometryOriginals"" AS f
		WHERE (f.""Data"" && {0}
			AND NOT f.""Data"" @ {0}) AND ST_Intersects(f.""Data"",  {0})";
    
    [Benchmark]
    public void EnumerateFragmentsSqlFullScreen()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, FullScreen)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlFullScreen()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, FullScreen)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsThreeLeftBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsThreeLeftBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsThreeLeftBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsThreeLeftBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsFiveTopBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsFiveTopBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsFiveTopBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsFiveTopBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsFiveTopAndFiveBottomBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsFiveTopAndFiveBottomBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsTwelveBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsTwelveBaikals)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsTwelveBaikals()
    {
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsTwelveBaikals)
		    .ToArray();
    }
}