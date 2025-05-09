using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Services.GeometryProviders.Interfaces;
using Utils;

namespace Services.GeometryProviders;

public class GeometryByScreenService : IGeometryByScreenService
{
    private GeometryDbContext _geometryDbContext;
    
    private static readonly string EnumerateFragmentsSql = 
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
                    SELECT *
                    FROM valid_intersections
            """;

    public GeometryByScreenService(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }
    
    public async Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(
        Polygon screen, double tolerance, CancellationToken cancellationToken)
    {
        return _geometryDbContext.Database.SqlQueryRaw<AreaIntersectionDto<Geometry>>(
            EnumerateFragmentsSql, screen, tolerance);
    }

}