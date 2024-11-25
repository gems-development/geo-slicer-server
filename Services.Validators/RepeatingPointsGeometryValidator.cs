using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.Validators;
using NetTopologySuite.Geometries;
using Services.ValidateErrors;
using Services.Validators.Interfaces;

namespace Services.Validators
{
    public class RepeatingPointsGeometryValidator : IConcreteValidator<Polygon>
    {
        private readonly ICoordinateComparator _coordinateComparator;

        public RepeatingPointsGeometryValidator(ICoordinateComparator coordinateComparator)
        {
            _coordinateComparator = coordinateComparator;
        }

        public GeometryValidateError ValidateGeometry(Polygon polygon)
        {
            RepeatingPointsValidator repeatingPointsValidator = new RepeatingPointsValidator(_coordinateComparator);

            LinearRing shell = polygon.Shell;
            LinearRing[] holes = polygon.Holes;

            if (!repeatingPointsValidator.IsValid(shell))
            {
                return GeometryValidateError.GeometryHasRepeatingPoints;
            }

            foreach (var hole in holes)
            {
                if (!repeatingPointsValidator.IsValid(hole))
                {
                    return GeometryValidateError.GeometryHasRepeatingPoints;
                }
            }

            return GeometryValidateError.GeometryValid;
        }
    }
}