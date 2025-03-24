using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Services.Interfaces;

public interface IAreaService<TGeometry> where TGeometry : Geometry
{
    Task<TGeometry> GetGeometryByRectangle(Point pointLeftBottom, Point pointRightTop);
}