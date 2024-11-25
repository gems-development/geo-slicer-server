using Services.ValidateErrors;

namespace Services.Fixers.Interfaces
{
    public interface IFixerFactory<TGeometry>
    {
        //Возвращает null в случае если на эту ошибку нет fixer
        public IConcreteFixer<TGeometry>? GetFixer(GeometryValidateError geometryValidateError);
    }
}