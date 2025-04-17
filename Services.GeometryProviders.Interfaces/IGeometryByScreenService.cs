using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace Services.GeometryProviders.Interfaces;

public interface IGeometryByScreenService
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(
        Polygon screen, double tolerance, CancellationToken cancellationToken);
}