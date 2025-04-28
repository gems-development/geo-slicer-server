using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers.Interfaces;

public abstract class IGeometryFixer<TGeometry> where TGeometry : Geometry
{
    public TGeometry FixGeometry(TGeometry geometry, GeometryValidateErrorType[] geometryValidateErrors)
    {
        if (geometryValidateErrors.All(error => error == GeometryValidateErrorType.GeometryValid))
            return geometry;
            
        geometryValidateErrors =
            geometryValidateErrors.Where(a => a != GeometryValidateErrorType.GeometryValid).ToArray();
            
        return Fix(geometry, geometryValidateErrors);
    }

    protected abstract TGeometry Fix(TGeometry geometry, GeometryValidateErrorType[] geometryValidateErrors);
}