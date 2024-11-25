using System.Collections.Generic;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using Services.Fixers.Interfaces;
using Services.ValidateErrors;

namespace Services.Fixers
{
    public class FixerFactory : IFixerFactory<Polygon>
    {
        private readonly IDictionary<GeometryValidateError, IConcreteFixer<Polygon>> _concreteFixers =
            new Dictionary<GeometryValidateError, IConcreteFixer<Polygon>>();

        public FixerFactory(double epsilon = 1e-14)
        {
            _concreteFixers[GeometryValidateError.GeometryHasRepeatingPoints] =
                new RepeatingPointsFixer(new EpsilonCoordinateComparator(epsilon));
        }

        public IConcreteFixer<Polygon>? GetFixer(GeometryValidateError geometryValidateError)
        {
            return _concreteFixers.TryGetValue(geometryValidateError, out var fixer) ? fixer : null;
        }
    }
}