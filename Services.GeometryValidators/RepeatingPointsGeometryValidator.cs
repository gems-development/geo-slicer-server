using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.Validators;
using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public class RepeatingPointsGeometryValidator : ISpecificValidator<Polygon>
{
    private readonly ICoordinateComparator _coordinateComparator;

    public RepeatingPointsGeometryValidator(double epsilon)
    {
        _coordinateComparator = new EpsilonCoordinateComparator(epsilon);
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
                Console.WriteLine(repeatingPointsValidator.GetErrorsString(hole));
                return GeometryValidateError.GeometryHasRepeatingPoints;
            }
        }

        return GeometryValidateError.GeometryValid;
    }
}