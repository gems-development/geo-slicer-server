using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace WebApp.UseCases.Repositories.Interfaces;

public interface IGeometryRepository<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByPolygonLinq(Polygon polygon, CancellationToken cancellationToken);
}