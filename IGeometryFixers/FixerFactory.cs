using GeometryValidateErrors;

namespace IGeometryFixers
{
    public class FixerFactory<TGeometry> : IFixerFactory<TGeometry>
    {
        public IConcreteFixer<TGeometry> GetFixer(GeometryValidateError geometryValidateError)
        {
            throw new System.NotImplementedException();
        }
    }
}