using System.Collections.Generic;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers
{
    public class GeometryGeometryFixerFactory : IGeometryFixerFactory<Polygon>
    {
        private readonly IDictionary<GeometryValidateError, ISpecificFixer<Polygon>> _concreteFixers =
            new Dictionary<GeometryValidateError, ISpecificFixer<Polygon>>();

        public GeometryGeometryFixerFactory(ICoordinateComparator epsilonCoordinateComparator)
        {
            _concreteFixers[GeometryValidateError.GeometryHasRepeatingPoints] =
                new RepeatingPointsFixer(epsilonCoordinateComparator);
        }

        public ISpecificFixer<Polygon>? GetFixer(GeometryValidateError geometryValidateError)
        {
            return _concreteFixers.TryGetValue(geometryValidateError, out var fixer) ? fixer : null;
        }
    }
}