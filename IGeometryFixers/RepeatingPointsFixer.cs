using System;
using GeometryValidateErrors;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using GeoSlicer.Utils.Validators;

namespace IGeometryFixers
{
    public class RepeatingPointsFixer : IConcreteFixer<Polygon>
    {
        private readonly ICoordinateComparator _coordinateComparator; 
        private readonly RepeatingPointsValidator _repeatingPointsValidator;
        private Func<Coordinate[], LinearRing> creator = array => new LinearRing(array);

        public RepeatingPointsFixer(ICoordinateComparator coordinateComparator)
        {
            _coordinateComparator = coordinateComparator;
            _repeatingPointsValidator = new RepeatingPointsValidator(_coordinateComparator);
        }
        
        public Polygon Fix(Polygon geometry)
        {
            LinearRing shell = geometry.Shell;
            LinearRing[] holes = geometry.Holes;

            LinearRing newShell = new LinearRing(_repeatingPointsValidator.Fix<>(shell, creator));
            LinearRing[] newHoles = new LinearRing[holes.Length];
            for (int i = 0; i < holes.Length; i++)
            {
                LinearRing hole = new LinearRing(_repeatingPointsValidator.Fix<>(holes[i], creator));
                newHoles[i] = hole;
            }

            return new Polygon(newShell, newHoles);
        }
    }
}