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
            GeometryValidateError[] validateErrors = new GeometryValidateError[_validators.Length];
            for (int i = 0; i < _validators.Length; i++)
            {
                validateErrors[i] = _validators[i].ValidateGeometry(geometry);
            }

            return validateErrors;
        }
    }
}