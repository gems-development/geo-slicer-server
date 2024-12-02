using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Valid;
using Services.ValidateErrors;
using Services.Validators.Interfaces;

namespace Services.Validators
{
    public class NetTopologySuiteValidatorAdapter<TGeometry> : IConcreteValidator<TGeometry> where TGeometry : Geometry
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
                catch (InvalidCastException e)
                {
                    return GeometryValidateError.UnknownError;
                }
            }

            return GeometryValidateError.GeometryValid;
        }
    }
}