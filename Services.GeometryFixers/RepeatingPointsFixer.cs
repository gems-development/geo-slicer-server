using System;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using GeoSlicer.Utils.Validators;
using Services.GeometryFixers.Interfaces;

namespace Services.GeometryFixers
{
    public class RepeatingPointsFixer : ISpecificFixer<Polygon>
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

            LinearRing newShell = _repeatingPointsValidator.Fix<LinearRing>(shell, creator);
            LinearRing[] newHoles = new LinearRing[holes.Length];
            for (int i = 0; i < holes.Length; i++)
            {
                LinearRing hole = _repeatingPointsValidator.Fix<LinearRing>(holes[i], creator);
                newHoles[i] = hole;
            }

            return new Polygon(newShell, newHoles);
        }
    }
}