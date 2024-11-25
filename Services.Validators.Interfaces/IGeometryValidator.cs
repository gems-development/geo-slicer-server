namespace Services.Interfaces
using NetTopologySuite.Geometries;
{
    public interface IGeometryValidator<TGeometry>
    {
        public GeometryValidateError[] ValidateGeometry(TGeometry geometry);
    }
}