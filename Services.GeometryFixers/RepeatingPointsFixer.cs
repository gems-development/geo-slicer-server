using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using GeoSlicer.Utils.Validators;
using Services.GeometryFixers.Interfaces;

namespace Services.GeometryFixers;

public class RepeatingPointsFixer : ISpecificFixer<Polygon>
{
    private readonly RepeatingPointsValidator _repeatingPointsValidator;
    private readonly Func<Coordinate[], LinearRing> _creator = array => new LinearRing(array);

    public RepeatingPointsFixer(double epsilon)
    {
        _repeatingPointsValidator = new RepeatingPointsValidator(new EpsilonCoordinateComparator(epsilon));
    }
        
    public Polygon Fix(Polygon geometry)
    {
        LinearRing shell = geometry.Shell;
        LinearRing[] holes = geometry.Holes;

        LinearRing newShell = _repeatingPointsValidator.Fix<LinearRing>(shell, _creator);
        LinearRing[] newHoles = new LinearRing[holes.Length];
        for (int i = 0; i < holes.Length; i++)
        {
            LinearRing hole = _repeatingPointsValidator.Fix<LinearRing>(holes[i], _creator);
            newHoles[i] = hole;
        }

        return new Polygon(newShell, newHoles);
    }
}