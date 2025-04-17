using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace Services.GeometryProviders.Interfaces;

public interface IGeometryByScreenService<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<TGeometry>>> GetGeometryByScreen(Polygon screen, CancellationToken cancellationToken);
}