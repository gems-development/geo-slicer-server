using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using NetTopologySuite.Geometries;
using GeoSlicer.Utils.Validators;

namespace Services.GeometryFixers;

//Исправляет подряд идущие повторяющиеся точки только в частях geometry, которые являются linearRing и lineString
public class RepeatingPointsFixer : SpecificTemplateFixer
{
    private readonly RepeatingPointsValidator _repeatingPointsValidator;
    private readonly Func<Coordinate[], LinearRing> _linearRingCreator = array => new LinearRing(array);
    private readonly Func<Coordinate[], LineString> _lineStringCreator = array => new LineString(array);

    public RepeatingPointsFixer(double epsilon)
    {
        _repeatingPointsValidator = new RepeatingPointsValidator(new EpsilonCoordinateComparator(epsilon));
    }
    
    protected override LinearRing FixLinearRing(LinearRing linearRing)
    {
        return _repeatingPointsValidator.Fix(linearRing, _linearRingCreator);
    }

    protected override LineString FixLineString(LineString lineString)
    {
        return _repeatingPointsValidator.Fix(lineString, _lineStringCreator);
    }
}