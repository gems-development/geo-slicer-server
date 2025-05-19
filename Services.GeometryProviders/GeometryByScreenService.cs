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
        	b."Id", "Alias" AS "LayerAlias", "Properties",
        	CASE 
               WHEN "Data" @ (SELECT geom FROM rectangle)
               THEN ST_Simplify("Data", (SELECT value FROM epsilon))
               ELSE ST_Intersection(ST_MakeValid(ST_Simplify("Data", (SELECT value FROM epsilon))), (SELECT geom FROM rectangle))
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