using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Services.GeometryProviders.Interfaces;

namespace Services.GeometryProviders;

public class GeometryByScreenService : IGeometryByScreenService
{
    private GeometryDbContext _geometryDbContext;
    
    private const string EnumerateFragmentsSql = """
                                                     WITH 	
                                                 	    originals_in_display AS MATERIALIZED (SELECT * FROM "GeometryOriginals" AS f WHERE f."Data" @ {0}),
                                                 	    fragments_in_display_ids AS MATERIALIZED  (
                                                 		    SELECT DISTINCT f."GeometryOriginalId" AS id
                                                             FROM "GeometryFragments" AS f 
                                                             WHERE f."Fragment" @ {0} AND NOT (f."GeometryOriginalId" = ANY(SELECT "Id" FROM originals_in_display))),
                                                         fragments_intersects_display_ids AS NOT MATERIALIZED (
                                                 		    SELECT DISTINCT f."GeometryOriginalId" AS id
                                                 		        FROM "GeometryFragments" AS f 
                                                 		        WHERE 
                                                 		    	    (f."Fragment" && {0} AND NOT f."Fragment" @ {0})
                                                 		    	    AND NOT (f."GeometryOriginalId" = ANY(SELECT id FROM fragments_in_display_ids))
                                                 		    	    AND ST_INTERSECTS(f."Fragment", {0}))			    	
                                                     SELECT f."Id", "Alias" AS "LayerAlias", "Properties", ST_MakeValid(ST_Simplify("Data", {1})) AS "Result" 
                                                        FROM originals_in_display AS f INNER JOIN "Layers" ON f."LayerId" = "Layers"."Id"
                                                     UNION ALL
                                                     SELECT f."Id", "Alias" AS "LayerAlias", "Properties", ST_Intersection(ST_MakeValid(ST_Simplify(f."Data", {1})), {0}) AS "Result"
                                                 	    FROM "GeometryOriginals" AS f INNER JOIN "Layers" ON f."LayerId" = "Layers"."Id" 
                                                 	    WHERE f."Id" = ANY(SELECT id FROM fragments_in_display_ids)
                                                     UNION ALL
                                                     SELECT f."Id", "Alias" AS "LayerAlias", "Properties", ST_Intersection(ST_MakeValid(ST_Simplify(f."Data", {1})), {0}) AS "Result"
                                                 	    FROM "GeometryOriginals" AS f INNER JOIN "Layers" ON f."LayerId" = "Layers"."Id"
                                                 	    WHERE f."Id" = ANY(SELECT id FROM fragments_intersects_display_ids)
                                                     
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
        
        
        
    //    var res = await _geometryDbContext.GeometryOriginals
     //       .Where(g => g.Data.Intersects(screen))
    //        .Select(g => 
    //            new AreaIntersectionDto<Geometry>(g.Id,
    //                g.Layer.Alias, g.Properties, g.Data.Intersection(screen)))
    //        .ToArrayAsync(cancellationToken: cancellationToken);
   //     return res;
    }

}