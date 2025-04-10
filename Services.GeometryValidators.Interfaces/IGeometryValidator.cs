using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;

namespace Services.GeometryValidators.Interfaces;

public interface IGeometryValidator<TGeometry> where TGeometry : Geometry
{
    public GeometryValidateError[] ValidateGeometry(TGeometry geometry);
}