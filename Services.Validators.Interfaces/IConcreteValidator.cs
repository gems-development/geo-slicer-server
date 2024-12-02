using Services.ValidateErrors;

namespace Services.Validators.Interfaces
{
    public interface IConcreteValidator<TGeometry>
    {
        GeometryValidateError ValidateGeometry(TGeometry polygon);
    }
}