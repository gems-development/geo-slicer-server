using NetTopologySuite.Geometries;

namespace WebAppUseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByLinearRing(LinearRing ring, CancellationToken cancellationToken);
}