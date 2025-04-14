using UseCases.Interfaces;
using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;
using Services.GeometryValidateErrors;
using Services.GeometryValidators.Interfaces;

namespace UseCases;

public class GeometryCorrector<TGeometryIn> : IGeometryCorrector<TGeometryIn> where TGeometryIn : Geometry
{
    private IGeometryValidator<TGeometryIn> GeometryValidator { get; set; }
    private IGeometryFixer<TGeometryIn> GeometryFixer { get; set; }
        
    public GeometryCorrector(
        IGeometryValidator<TGeometryIn> geometryValidator, 
        IGeometryFixer<TGeometryIn> geometryFixer)
    {
        GeometryValidator = geometryValidator;
        GeometryFixer = geometryFixer;
    }
    public bool ValidateGeometry(
        ref GeometryValidateError[]? geometryValidateErrors, TGeometryIn geometry, ref string result)
    {
        geometryValidateErrors = GeometryValidator.ValidateGeometry(geometry);
        result = result + "Validate errors: " + string.Join("\n", geometryValidateErrors);
        return true;
    }

    public TGeometryIn FixGeometry(
        bool validatedGeometry, TGeometryIn geometry, GeometryValidateError[]? geometryValidateErrors)
    {
        if (!validatedGeometry)
            throw new ApplicationException("geometry was not validated, but a fix parameter was sent");
        geometry = GeometryFixer.FixGeometry(geometry, geometryValidateErrors!);
        return geometry;
    }
}