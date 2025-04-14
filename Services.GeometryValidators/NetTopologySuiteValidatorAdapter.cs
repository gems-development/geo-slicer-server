using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Valid;
using Services.GeometryValidateErrors;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public class NetTopologySuiteValidatorAdapter<TGeometry> : ISpecificValidator<TGeometry> where TGeometry : Geometry
{
    public GeometryValidateError ValidateGeometry(TGeometry geometry)
    {
        var validateOp = new IsValidOp(geometry);
        if (!validateOp.IsValid)
        {
            try
            {
                return (GeometryValidateError) ((int) validateOp.ValidationError.ErrorType);
            }
            catch (InvalidCastException)
            {
                return GeometryValidateError.UnknownError;
            }
        }

        return GeometryValidateError.GeometryValid;
    }
}