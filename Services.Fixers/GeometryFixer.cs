using System;
using System.Collections.Generic;
using Services.Fixers.Interfaces;
using Services.ValidateErrors;

namespace Services.Fixers
{
    public class GeometryFixer<TGeometry> : IGeometryFixer<TGeometry>
    {
        private IFixerFactory<TGeometry> _fixerFactory;

        public GeometryFixer(IFixerFactory<TGeometry> fixerFactory)
        {
            _fixerFactory = fixerFactory;
        }

        protected override TGeometry Fix(TGeometry geometry, GeometryValidateError[] geometryValidateErrors)
        {
            foreach (var error in geometryValidateErrors)
            {
                var fixer = _fixerFactory.GetFixer(error);
                if (fixer == null)
                    throw new ApplicationException($"There is no fixer for the error {error}");
                geometry = fixer.Fix(geometry);
            }
            return geometry;
        }
    }
}