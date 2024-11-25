using GeometryValidateErrors;
using NetTopologySuite.Geometries;

namespace IGeometryValidators
{
    public interface IGeometryValidator<TGeometry>
    {
        public GeometryValidateError[] ValidateGeometry(TGeometry geometry);
    }
}