using Services.GeometryValidateErrors;

namespace Services.GeometryFixers.Interfaces
{
    public interface IGeometryFixerFactory<TGeometry>
    {
        //Возвращает null в случае если на эту ошибку нет fixer
        public ISpecificFixer<TGeometry>? GetFixer(GeometryValidateError geometryValidateError);
    }
}