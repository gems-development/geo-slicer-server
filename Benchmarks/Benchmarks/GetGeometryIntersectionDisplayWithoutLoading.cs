using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Services.ScaleCalculator;
using Utils;

namespace Benchmarks.Benchmarks;

//todo бенчмарк с актуальными подходами для поиска геометрий для рендеринга без учета времени на их выгрузку и обработку

//подробное описание в GetGeometryIntersectionDisplay

public class GetGeometryIntersectionDisplayWithoutLoading
{
	private static readonly PostgreApplicationContext PgContext =
		new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");
    private Polygon FullScreen { get; set; }
    
    private Polygon ScreenIntersectsThreeLeftBaikals { get; set; }
    
    private Polygon ScreenIntersectsTwelveBaikals { get; set; }
    
    private Polygon ScreenIntersectsFiveTopBaikals { get; set; }
    
    private Polygon ScreenIntersectsFiveTopAndFiveBottomBaikals { get; set; }
    private Polygon Screen145IntersectsBaikals { get; set; }

    private LinearToleranceCalculator _toleranceCalculator;
    
    public GetGeometryIntersectionDisplayWithoutLoading()
    {
	    _toleranceCalculator = new LinearToleranceCalculator(5E-4);
	    
        Coordinate leftBottom = new Coordinate(96.480, 44.269);
        Coordinate rightTop = new Coordinate(116.940, 62.598); 
        
        FullScreen = CreateRectanglePolygon(leftBottom, rightTop, GeometryServerSrid.Srid);
        
        leftBottom = new Coordinate(100.051, 45.041);
        rightTop = new Coordinate(117.107, 62.670); 
        
        ScreenIntersectsThreeLeftBaikals = CreateRectanglePolygon(leftBottom, rightTop, GeometryServerSrid.Srid);
        
        leftBottom = new Coordinate(99.4070, 46.4849);
        rightTop = new Coordinate(114.1381, 60.5477); 
        
        ScreenIntersectsTwelveBaikals = CreateRectanglePolygon(leftBottom, rightTop, GeometryServerSrid.Srid);
        
        leftBottom = new Coordinate(96.480, 44.269);
        rightTop = new Coordinate(116.046, 56.63072);
        
        ScreenIntersectsFiveTopBaikals = CreateRectanglePolygon(leftBottom, rightTop, GeometryServerSrid.Srid);
        
        leftBottom = new Coordinate(96.480, 50.56485);
        rightTop = new Coordinate(116.046, 56.63072);
        
        ScreenIntersectsFiveTopAndFiveBottomBaikals = CreateRectanglePolygon(leftBottom, rightTop, GeometryServerSrid.Srid);
        
        
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(GeometryServerSrid.Srid);


        var coordinates = new Coordinate[]
        {
	        new Coordinate(72.0257, 41.3670),   // Нижняя левая
	        new Coordinate(136.94639, 41.3670), // Нижняя правая
	        new Coordinate(136.94639, 61.78870), // Верхняя правая
	        new Coordinate(72.0257, 61.78870),  // Верхняя левая
	        new Coordinate(72.0257, 41.3670)    // Замыкаем полигон
        };


        var ring = geometryFactory.CreateLinearRing(coordinates);


	    Screen145IntersectsBaikals = geometryFactory.CreatePolygon(ring);

        Screen145IntersectsBaikals.SRID = GeometryServerSrid.Srid;
    }

    private static Polygon CreateRectanglePolygon(Coordinate leftBottom, Coordinate rightTop, int srid)
    {
        var polygon = new GeometryFactory().CreatePolygon(new[]
        {
            new Coordinate(leftBottom.X, leftBottom.Y),
            new Coordinate(rightTop.X, leftBottom.Y),
            new Coordinate(rightTop.X, rightTop.Y),    
            new Coordinate(leftBottom.X, rightTop.Y),   
            new Coordinate(leftBottom.X, leftBottom.Y) 
        });
        polygon.SRID = srid;
        return polygon;
    }
    
    private string _bbSql = 
	     """
	     BEGIN;
	     SET LOCAL max_parallel_workers_per_gather = 0;
	     WITH rectangle AS MATERIALIZED
	     (
	     	SELECT {0} AS geom
	     ),
	     epsilon AS MATERIALIZED
	     (
	     	SELECT {1} AS value
	     )
	     SELECT 
	     	/*b."Id", "Alias" AS "LayerAlias", "Properties",*/ 
	     	CASE 
	            WHEN "Data" @ (SELECT geom FROM rectangle)
	            THEN /*ST_Simplify("Data", (SELECT value FROM epsilon))*/ 1
	            ELSE /*ST_Intersection(ST_MakeValid(ST_Simplify("Data", (SELECT value FROM epsilon))), (SELECT geom FROM rectangle))*/ 1
	         END AS "Result"
	         
	     FROM "GeometryOriginals" AS b INNER JOIN "Layers" ON "LayerId" = "Layers"."Id"
	     WHERE b."Data" && (SELECT geom FROM rectangle) AND 
	     (
	     	b."Data" @ (SELECT geom FROM rectangle)
	     	OR
	     	(
	     		EXISTS
	     		(
	     	        
	             	SELECT 1 FROM "GeometryFragments" AS f 
	             	WHERE f."GeometryOriginalId" = b."Id" AND f."Fragment" @ (SELECT geom FROM rectangle)
	     	        LIMIT 1
	     	    )
	     	    OR
	     	    EXISTS
	     		(
	     	        SELECT 1 FROM 
	     	        	(SELECT f."Fragment" AS fragment FROM "GeometryFragments" AS f 
	     	        	WHERE f."GeometryOriginalId" = b."Id" AND f."Fragment" && (SELECT geom FROM rectangle)) as h
	     	        WHERE (ST_INTERSECTS(h.fragment, (SELECT geom FROM rectangle)))
	     	        LIMIT 1
	     	    )
	         )               
	     );
	     COMMIT
	     """;

    private string _enumerateOriginalsSql = @"
		BEGIN;
		SET LOCAL max_parallel_workers_per_gather = 0;
		SELECT /*ST_Simplify(""Data"", {1}) AS ""Data""*/ 1
		FROM ""GeometryOriginals"" AS f
		WHERE f.""Data"" @ {0}
		UNION ALL 
		SELECT /*ST_Intersection(ST_MakeValid(ST_Simplify(""Data"", {1})), {0}) AS ""Data""*/ 1
		FROM ""GeometryOriginals"" AS f
		WHERE (f.""Data"" && {0}
			AND NOT f.""Data"" @ {0}) AND ST_Intersects(f.""Data"", {0});
		COMMIT";
    
    [Benchmark]
    public void EnumerateBbSqlFullScreen()
    {
	    var points = GetBoundingBox(FullScreen);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_bbSql, FullScreen, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlFullScreen()
    {
	    var points = GetBoundingBox(FullScreen);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_enumerateOriginalsSql, FullScreen, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsThreeLeftBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsThreeLeftBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_bbSql, ScreenIntersectsThreeLeftBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsThreeLeftBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsThreeLeftBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_enumerateOriginalsSql, ScreenIntersectsThreeLeftBaikals, tolerance)
		    .ToArray();
    }
    
    
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsFiveTopBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_bbSql, ScreenIntersectsFiveTopBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsFiveTopBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_enumerateOriginalsSql, ScreenIntersectsFiveTopBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopAndFiveBottomBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_bbSql, ScreenIntersectsFiveTopAndFiveBottomBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopAndFiveBottomBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_enumerateOriginalsSql, ScreenIntersectsFiveTopAndFiveBottomBaikals, tolerance)
		    .ToArray();
    }
    
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsTwelveBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsTwelveBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_bbSql, ScreenIntersectsTwelveBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsTwelveBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsTwelveBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_enumerateOriginalsSql, ScreenIntersectsTwelveBaikals, tolerance)
		    .ToArray();
    }
		
    public static (Point BottomLeft, Point TopRight) GetBoundingBox(Polygon polygon)
    {
	    if (polygon == null || polygon.IsEmpty)
	    {
		    throw new ArgumentException("Полигон не может быть пустым или null.");
	    }

	    // Создаём фабрику для геометрий
	    var geometryFactory = new GeometryFactory();

	    // Получаем ограничивающий прямоугольник (Envelope) полигона
	    var envelope = polygon.EnvelopeInternal;

	    // Левая нижняя точка: минимальные X и Y
	    Point bottomLeft = geometryFactory.CreatePoint(new Coordinate(envelope.MinX, envelope.MinY));
	    // Правая верхняя точка: максимальные X и Y
	    Point topRight = geometryFactory.CreatePoint(new Coordinate(envelope.MaxX, envelope.MaxY));

	    return (bottomLeft, topRight);
    }
    
    //тест на 145 фигурах (figure1, figure2, ... figure144, baikal)
    
    /*[Benchmark]
    public void EnumerateBbSqlScreenIntersects145Baikals()
    {
	    var points = GetBoundingBox(Screen145IntersectsBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_bbSql, Screen145IntersectsBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersects145Baikals()
    {
	    var points = GetBoundingBox(Screen145IntersectsBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<int>(_enumerateOriginalsSql, Screen145IntersectsBaikals, tolerance)
		    .ToArray();
    }*/
}