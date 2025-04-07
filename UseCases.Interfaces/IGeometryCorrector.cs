using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;

namespace UseCases.Interfaces;

public interface IGeometryCorrector<TGeometryIn> where TGeometryIn : Geometry
{
    bool ValidateGeometry(ref GeometryValidateError[]? geometryValidateErrors, TGeometryIn geometry, ref string result);

    TGeometryIn FixGeometry(bool validatedGeometry, TGeometryIn geometry, GeometryValidateError[]? geometryValidateErrors);
}