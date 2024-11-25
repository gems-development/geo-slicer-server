using Services.ValidateErrors;

namespace Services.Validators.Interfaces
{
    public interface IConcreteValidator<TGeometry>
    {
        public GeometryValidateError ValidateGeometry(TGeometry polygon);
    }
}