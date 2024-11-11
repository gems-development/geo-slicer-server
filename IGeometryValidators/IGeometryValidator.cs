using GeometryValidateErrors;

namespace IGeometryValidators
{
    public interface IGeometryValidator<TGeometry>
    {
        public GeometryValidateError ValidateGeometry(TGeometry geometry);
    }
}