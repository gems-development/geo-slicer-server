using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace Services.GeometryProviders.Interfaces;

public interface IGeometryByScreenService<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(Polygon screen, CancellationToken cancellationToken);
}