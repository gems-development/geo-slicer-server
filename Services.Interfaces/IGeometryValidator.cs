namespace Services.Interfaces
{
    public interface IGeometryValidator<TGeometry>
    {
        public GeometryValidateError ValidateGeometry(TGeometry geometry);
    }
}