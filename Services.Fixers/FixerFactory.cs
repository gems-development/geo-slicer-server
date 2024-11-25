using Services.Fixers.Interfaces;
using Services.ValidateErrors;

namespace Services.Fixers
{
    public class FixerFactory<TGeometry> : IFixerFactory<TGeometry>
    {
        public IConcreteFixer<TGeometry> GetFixer(GeometryValidateError geometryValidateError)
        {
            throw new System.NotImplementedException();
        }
    }
}