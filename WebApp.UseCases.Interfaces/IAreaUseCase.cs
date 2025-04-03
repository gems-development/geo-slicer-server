using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace WebApp.UseCases.Interfaces;

public interface IAreaUseCase<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken);
}