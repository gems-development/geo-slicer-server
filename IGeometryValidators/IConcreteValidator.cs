using GeometryValidateErrors;

namespace IGeometryValidators
{
    public interface IConcreteValidator<TGeometry>
    {
        public GeometryValidateError ValidateGeometry(TGeometry polygon);
    }
}