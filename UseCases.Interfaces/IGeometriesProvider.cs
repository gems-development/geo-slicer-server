using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Point = NetTopologySuite.Geometries.Point;

namespace UseCases.Interfaces;

public interface IGeometriesProvider<TGeometry> where TGeometry : Geometry
{
    Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken);
}