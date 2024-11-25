using Services.ValidateErrors;

namespace Services.Fixers.Interfaces
{
    public interface IFixerFactory<TGeometry>
    {
        public IConcreteFixer<TGeometry> GetFixer(GeometryValidateError geometryValidateError);
    }
}