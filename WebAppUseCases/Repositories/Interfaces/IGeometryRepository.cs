using NetTopologySuite.Geometries;

namespace WebAppUseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByPolygon(Polygon ring);
    Task<TGeometry> GetSimplifiedGeometryByPolygon(Polygon polygon, double tolerance);
}