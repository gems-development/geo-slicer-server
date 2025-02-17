using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using Services.ValidateErrors;
using Services.Validators.Interfaces;

namespace Services.Validators
{
    public class GeometryValidator<TGeometry> : IGeometryValidator<TGeometry>
    {
        private IConcreteValidator<TGeometry>[] _validators;
        
        public GeometryValidator(IEnumerable<IConcreteValidator<TGeometry>> validators)
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
                    validateErrors.Add(_validators[i].ValidateGeometry(geometry));
                }
            }
            if (!validateErrors.Any())
            {
                validateErrors.Add(GeometryValidateError.GeometryValid);
            }
            return validateErrors.ToArray();
        }
    }
}