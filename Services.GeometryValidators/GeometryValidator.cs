using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public class GeometryValidator<TGeometry> : IGeometryValidator<TGeometry> where TGeometry : Geometry
{
    private ISpecificValidator<TGeometry>[] _validators;
        
    public GeometryValidator(IEnumerable<ISpecificValidator<TGeometry>> validators)
    {
        _validators = validators.ToArray();
    }

    public GeometryValidateError[] ValidateGeometry(TGeometry geometry)
    {
        HashSet<GeometryValidateError> validateErrors = new HashSet<GeometryValidateError>(_validators.Length);
        for (int i = 0; i < _validators.Length; i++)
        {
            var answer = _validators[i].ValidateGeometry(geometry);
            if (answer != GeometryValidateError.GeometryValid)
            {
                validateErrors.Add(answer);
            }
        }
        if (!validateErrors.Any())
        {
            validateErrors.Add(GeometryValidateError.GeometryValid);
        }
        return validateErrors.ToArray();
    }
}