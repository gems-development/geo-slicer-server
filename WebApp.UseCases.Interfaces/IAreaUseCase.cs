using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Point = NetTopologySuite.Geometries.Point;

namespace WebApp.UseCases.Interfaces;

public interface IAreaUseCase<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken);
}