using Services.ValidateErrors;

namespace Services.Validators.Interfaces
{
    public interface IGeometryValidator<TGeometry>
    {
        public GeometryValidateError[] ValidateGeometry(TGeometry geometry);
    }
}