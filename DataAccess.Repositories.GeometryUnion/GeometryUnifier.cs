using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;

namespace DataAccess.Repositories.GeometryUnion;

public static class GeometryUnifier
{
    public static Polygon Union(IEnumerable<Polygon> objectsToUnify)
    {
        try
        {
            var result = (Polygon)UnaryUnionOp.Union(objectsToUnify);
            return result;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error while unifying polygons: " + ex.Message, ex);
        }
    }
}