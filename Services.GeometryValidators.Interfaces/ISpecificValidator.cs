using Services.GeometryValidateErrors;

namespace Services.GeometryValidators.Interfaces
{
    public interface ISpecificValidator<TGeometry>
    {
        GeometryValidateError ValidateGeometry(TGeometry polygon);
    }
}