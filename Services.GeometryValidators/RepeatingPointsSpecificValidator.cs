using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.Validators;
using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;

namespace Services.GeometryValidators;

public class RepeatingPointsSpecificValidator : SpecificTemplateValidator
{
    private readonly RepeatingPointsValidator _repeatingPointsValidator;

    public RepeatingPointsSpecificValidator(double epsilon)
    {
        _repeatingPointsValidator = new RepeatingPointsValidator(new EpsilonCoordinateComparator(epsilon));
    }

    protected override GeometryValidateError ValidateLineString(LineString lineString)
    {
        if (!_repeatingPointsValidator.IsValid(lineString))
        {
            return new GeometryValidateError(
                GeometryValidateErrorType.GeometryHasRepeatingPoints,
                _repeatingPointsValidator.GetErrorsString(lineString));
        }

        return GeometryValid;
    }
    
    protected override GeometryValidateError ValidateLinearRing(LinearRing linearRing)
    {
        return ValidateLineString(linearRing);
    }
}