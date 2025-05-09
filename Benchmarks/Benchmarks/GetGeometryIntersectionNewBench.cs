using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Services.ScaleCalculator;
using Utils;

namespace Benchmarks.Benchmarks;

//todo бенчмарк с актуальными подходами для поиска геометрий для рендеринга

//бенчмарк должен проводится в связке с бд, в которой сохранены figure1, figure2, ... figure14 и baikal из папки TestFiles
//(кроме последних 3 методов в бенчмарке)
//бенчмарк проводился на фигурах, фрагменты которых имеют не более 1000 точек

//Проблема: _bbSql быстрее других методов на 15 фигурах, однако на 145 фигурах _enumerateOriginalsSql все равно быстрее
//(последние 3 метода в бенчмарке)

//_bbSql имеет потенциальную проблему: он считает, что фигура, bbox которой пересекает своей одной гранью две параллельные
//грани входного прямоугольника(экрана), пересекает этот экран(хотя сама фигура может не пересекаться фактически с входным
//прямоугольником). Это проблема может повлиять на производительность, так как на вход ST_Intersection может попасть фигура,
//которая не пересекается с экраном(а смысл всех этих запросов не подавать на вход ST_Intersection фигуры, которые не пересекают экран).

//так же в _bbSql не выполняется поиск по фрагментам если входной прямоугольник(экран) содержится в bbox фигуры, сразу вызывается ST_Intersection

//Возможность для оптимизации - сохранять заранее в бд упрощенные фигуры с помощью ST_SimplifyPreserveTopology, а не упрощать на лету

public class GetGeometryIntersectionNewBench
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
    
    public GetGeometryIntersectionNewBench()
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
    
    private string _enumerateFragmentsSql = @"
    WITH 	
	    originals_in_display AS MATERIALIZED (SELECT * FROM ""GeometryOriginals"" AS f WHERE f.""Data"" @ {0}),
	    fragments_bb_intersects_display_ids AS NOT MATERIALIZED  (
		    SELECT f.""GeometryOriginalId"" AS id, f.""Fragment"" AS fragment
            FROM ""GeometryFragments"" AS f 
            WHERE f.""Fragment"" && {0} AND NOT (f.""GeometryOriginalId"" = ANY(SELECT ""Id"" FROM originals_in_display))),
        fragments_intersects_display_ids AS NOT MATERIALIZED (
		    SELECT f.id
		        FROM fragments_bb_intersects_display_ids AS f
				GROUP BY f.id
				HAVING BOOL_OR(f.fragment @ {0} OR ST_INTERSECTS(f.fragment, {0})))			    	
    SELECT ST_MakeValid(ST_Simplify(""Data"", {1})) FROM originals_in_display AS ""Data""
    UNION ALL
    SELECT ST_Intersection(ST_MakeValid(ST_Simplify(f.""Data"", {1})), {0}) AS ""Data""
	    FROM ""GeometryOriginals"" AS f
	    WHERE f.""Id"" = ANY(
			SELECT id FROM fragments_intersects_display_ids)
    ";
	//старая версия запроса
    /*private string _enumerateFragmentsSql = @"
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
	    WHERE f.""Id"" = ANY(
			SELECT id FROM fragments_in_display_ids 
				UNION ALL 
			SELECT id FROM fragments_intersects_display_ids)
    ";*/
    
    //sql запрос, который ищет фигуры, bb которых пересекает либо одну грань прямоугольника, либо две параллельных грани
    //прямоугольника. Фигуры, bb которых пересекает две непараллельных грани прямоугольника, проверяются по фрагментам
    private string _bbSql = 
	     """
	    	WITH SRID AS (
	    	    SELECT 
	    """ + GeometryServerSrid.Srid + """
             AS srid
                    ),
                    input_rect AS (
                        -- Входной прямоугольник с заданным SRID
                        SELECT {0} AS geom
                    ),
                    -- Извлекаем вершины прямоугольника
                    rect_points AS (
                        SELECT (ST_DumpPoints(geom)).geom AS point
                        FROM input_rect
                    ),
                    -- Формируем грани прямоугольника (4 линии)
                    rect_edges AS (
                        SELECT ST_SetSRID(ST_MakeLine(point, LEAD(point) OVER ()), (SELECT srid FROM SRID)) AS edge
                        FROM rect_points
                        WHERE point IS NOT NULL
                        LIMIT 4
                    ),
                    -- Нумеруем грани и определяем их тип (0 - горизонтальные, 1 - вертикальные)
                    edges AS (
                        SELECT 
                            edge,
                            CASE 
                                WHEN ST_Y(ST_StartPoint(edge)) = ST_Y(ST_EndPoint(edge)) THEN 0 -- горизонтальная
                                ELSE 1 -- вертикальная
                            END AS edge_type
                        FROM rect_edges
                    ),
                    -- Находим bounding box'ы фигур и их пересечения с гранями
                    bbox_intersections AS (
                        SELECT 
                            g."Data",
                            g."Id",
                            g."Properties",
                            g."LayerId",
                            e.edge_type
                        FROM (SELECT * FROM "GeometryOriginals" s WHERE (s."Data" && {0} AND NOT s."Data" @ {0})) g
                        CROSS JOIN edges e
                        WHERE g."Data" && e.edge AND NOT e.edge @ g."Data"
                    ),
                    -- Подсчитываем количество пересечений и проверяем условия
                    valid_intersections AS (
                        SELECT 
                            b."Id", ANY_VALUE("Alias") AS "LayerAlias", ANY_VALUE("Properties") AS "Properties", ST_Intersection(ST_MakeValid(ST_Simplify(ANY_VALUE("Data"), {1})), {0}) AS "Result"
                        FROM bbox_intersections b INNER JOIN "Layers" ON "LayerId" = "Layers"."Id"
                        GROUP BY b."Id"
                        HAVING 
                            -- Ровно одно пересечение
                            COUNT(*) = 1
                            OR 
                            -- Два пересечения, но только параллельные грани (одинаковый edge_type)
                            (COUNT(*) = 2 AND array_length(ARRAY_AGG(DISTINCT edge_type), 1) = 1)
                            OR EXISTS (
                                SELECT 1 FROM "GeometryFragments" AS f 
                                WHERE f."GeometryOriginalId" = b."Id" AND f."Fragment" && {0}
                                GROUP BY f."GeometryOriginalId"
                                HAVING BOOL_OR(f."Fragment" @ {0} OR ST_INTERSECTS(f."Fragment", {0}))
                            )
                        UNION ALL
                            SELECT b."Id", "Alias" AS "LayerAlias", "Properties", ST_MakeValid(ST_Simplify("Data", {1})) AS "Result"
                            FROM "GeometryOriginals" b INNER JOIN "Layers" ON "LayerId" = "Layers"."Id"
                            WHERE "Data" @ {0}
                        UNION ALL  
                            SELECT b."Id", "Alias" AS "LayerAlias", "Properties", ST_Intersection(ST_MakeValid(ST_Simplify("Data", {1})), {0}) AS "Result"
                            FROM "GeometryOriginals" b INNER JOIN "Layers" ON "LayerId" = "Layers"."Id"
                            WHERE {0} @ "Data"
                    )
                    SELECT "Result"
                    FROM valid_intersections
            """;

    private string _enumerateOriginalsSql = @"
		SELECT ST_MakeValid(ST_Simplify(""Data"", {1})) AS ""Data""
		FROM ""GeometryOriginals"" AS f
		WHERE f.""Data"" @ {0}
		UNION ALL 
		SELECT ST_Intersection(ST_MakeValid(ST_Simplify(""Data"", {1})), {0}) AS ""Data""
		FROM ""GeometryOriginals"" AS f
		WHERE (f.""Data"" && {0}
			AND NOT f.""Data"" @ {0}) AND ST_Intersects(f.""Data"",  {0})";
    [Benchmark]
    public void EnumerateFragmentsSqlFullScreen()
    {
	    var points = GetBoundingBox(FullScreen);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, FullScreen, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlFullScreen()
    {
	    var points = GetBoundingBox(FullScreen);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_bbSql, FullScreen, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlFullScreen()
    {
	    var points = GetBoundingBox(FullScreen);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, FullScreen, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsThreeLeftBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsThreeLeftBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsThreeLeftBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsThreeLeftBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsThreeLeftBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_bbSql, ScreenIntersectsThreeLeftBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsThreeLeftBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsThreeLeftBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsThreeLeftBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsFiveTopBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsFiveTopBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsFiveTopBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_bbSql, ScreenIntersectsFiveTopBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsFiveTopBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsFiveTopBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopAndFiveBottomBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsFiveTopAndFiveBottomBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopAndFiveBottomBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_bbSql, ScreenIntersectsFiveTopAndFiveBottomBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsFiveTopAndFiveBottomBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsFiveTopAndFiveBottomBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsFiveTopAndFiveBottomBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateFragmentsSqlScreenIntersectsTwelveBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsTwelveBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, ScreenIntersectsTwelveBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersectsTwelveBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsTwelveBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_bbSql, ScreenIntersectsTwelveBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersectsTwelveBaikals()
    {
	    var points = GetBoundingBox(ScreenIntersectsTwelveBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, ScreenIntersectsTwelveBaikals, tolerance)
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
    
    //поиск по оригиналам _enumerateOriginalsSql быстрее поисков по фрагментам
    
    /*[Benchmark]
    public void EnumerateFragmentsSqlScreenIntersects145Baikals()
    {
	    var points = GetBoundingBox(Screen145IntersectsBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateFragmentsSql, Screen145IntersectsBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateBbSqlScreenIntersects145Baikals()
    {
	    var points = GetBoundingBox(Screen145IntersectsBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_bbSql, Screen145IntersectsBaikals, tolerance)
		    .ToArray();
    }
    
    [Benchmark]
    public void EnumerateOriginalsSqlScreenIntersects145Baikals()
    {
	    var points = GetBoundingBox(Screen145IntersectsBaikals);
	    
	    var tolerance = _toleranceCalculator.CalculateTolerance(points.BottomLeft, points.TopRight);
	    var res = PgContext.Database
		    .SqlQueryRaw<Geometry>(_enumerateOriginalsSql, Screen145IntersectsBaikals, tolerance)
		    .ToArray();
    }*/
}