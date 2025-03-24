using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByPolygonLinq(Polygon ring);
    Task<TGeometry> GetSimplifiedGeometryByPolygon(Polygon polygon, double tolerance);
    Task<Geometry> GetGeometryByPolygonEnumerateFragments(Polygon polygon);
}