using NetTopologySuite.Geometries;

namespace WebAppUseCases.Services.Interfaces;

public interface IAreaService<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken);
}