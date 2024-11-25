using System;
using System.Collections.Generic;
using Services.Fixers.Interfaces;
using Services.ValidateErrors;

namespace Services.Fixers
{
    public class GeometryFixer<TGeometry> : IGeometryFixer<TGeometry>
    {
        private IDictionary<GeometryValidateError, Func<TGeometry, TGeometry>> Fixers { get; set; }

        public GeometryFixer(IDictionary<GeometryValidateError, Func<TGeometry, TGeometry>> fixers)
        {
            Fixers = fixers;
        }

        protected override TGeometry Fix(TGeometry geometry, GeometryValidateError[] geometryValidateErrors)
        {
            foreach (var error in geometryValidateErrors)
            {
                bool result = Fixers.TryGetValue(error, out var fixer);
                if (!result)
                    throw new ApplicationException($"no fixer was found for this error: {error}");
                geometry = fixer!(geometry);
            }

            return geometry;
        }
    }
}