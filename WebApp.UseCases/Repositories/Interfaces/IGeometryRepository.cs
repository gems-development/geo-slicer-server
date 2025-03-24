using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByLinearRing(LinearRing ring, CancellationToken cancellationToken);
}