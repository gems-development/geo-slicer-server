using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;

namespace DataAccess.Repositories.GeometryUnion;

public static class GeometryUnifier
{
    public static Geometry Union(IEnumerable<Polygon> objectsToUnify)
    {
        try
        {
            return (MultiPolygon)UnaryUnionOp.Union(objectsToUnify);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error while unifying polygons: " + ex.Message, ex);
        }
    }
}