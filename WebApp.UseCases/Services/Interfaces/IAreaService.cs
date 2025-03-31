using NetTopologySuite.Geometries;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Services.Interfaces;

public interface IAreaService<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken);
}