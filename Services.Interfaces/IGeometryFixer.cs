namespace Services.Interfaces
{
    public abstract class IGeometryFixer<TGeometry>
    {
        public TGeometry FixGeometry(TGeometry geometry, GeometryValidateError geometryValidateError)
        {
            if (geometryValidateError == GeometryValidateError.GeometryValid)
                return geometry;
            return Fix(geometry, geometryValidateError);
        }

        protected abstract TGeometry Fix(TGeometry geometry, GeometryValidateError geometryValidateError);
    }
}