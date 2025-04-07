using Services.GeometryValidateErrors;

namespace Services.GeometryValidators.Interfaces
{
    public interface IGeometryValidator<TGeometry>
    {
        public GeometryValidateError[] ValidateGeometry(TGeometry geometry);
    }
}