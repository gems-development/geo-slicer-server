using NetTopologySuite.Geometries;

namespace WebAppUseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop);
}