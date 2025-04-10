using NetTopologySuite.Geometries;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers.Interfaces;

public interface IGeometryFixerFactory<TGeometry> where TGeometry : Geometry
{
    //Возвращает null в случае если на эту ошибку нет fixer
    public ISpecificFixer<TGeometry>? GetFixer(GeometryValidateError geometryValidateError);
}