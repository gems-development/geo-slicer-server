using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;

namespace Services.GeometryValidators.Interfaces;

public interface ISpecificValidator<TGeometry> where TGeometry : Geometry
{
    GeometryValidateError ValidateGeometry(TGeometry polygon);
}