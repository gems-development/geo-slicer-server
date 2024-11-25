using GeometryValidateErrors;
using NetTopologySuite.Geometries;

namespace IGeometryValidators
{
    public class GeometryValidator : IGeometryValidator<Polygon>
    {
        private IConcreteValidator<Polygon>[] _validators;
        
        public GeometryValidator(IConcreteValidator<Polygon>[] validators)
        {
            _validators = validators;
        }

        public GeometryValidateError[] ValidateGeometry(Polygon geometry)
        {
            GeometryValidateError[] validateErrors = new GeometryValidateError[_validators.Length];
            for (int i = 0; i < _validators.Length; i++)
            {
                validateErrors[i] = _validators[i].ValidateGeometry(geometry);
            }

            return validateErrors;
        }
    }
}