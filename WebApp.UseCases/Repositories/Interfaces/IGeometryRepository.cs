using NetTopologySuite.Geometries;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByLinearRing(LinearRing ring, CancellationToken cancellationToken);
}