using GeometryValidateErrors;

namespace IGeometryFixers
{
    public interface IFixerFactory<TGeometry>
    {
        public IConcreteFixer<TGeometry> GetFixer(GeometryValidateError geometryValidateError);
    }
}