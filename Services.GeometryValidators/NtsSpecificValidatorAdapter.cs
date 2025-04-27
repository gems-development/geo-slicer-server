using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Valid;
using Services.GeometryValidateErrors;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public class NtsSpecificValidatorAdapter : ISpecificValidator<Geometry>
{
    public GeometryValidateError ValidateGeometry(Geometry geometry)
    {
        var validateOp = new IsValidOp(geometry);
        if (!validateOp.IsValid)
        {
            try
            {
                return new GeometryValidateError(
                    (GeometryValidateErrorType) (int) validateOp.ValidationError.ErrorType,
                    validateOp.ValidationError.Message);
            }
            catch (InvalidCastException)
            {
                return new GeometryValidateError(
                    GeometryValidateErrorType.UnknownError,
                    validateOp.ValidationError.Message);
            }
        }

        return new GeometryValidateError(GeometryValidateErrorType.GeometryValid, "no errors were found");
    }
}