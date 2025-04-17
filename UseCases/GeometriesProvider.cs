using UseCases.Interfaces;
using NetTopologySuite.Geometries;
using Services.GeometryProviders.Interfaces;
using Utils;
using WebApp.Dto.Responses;

namespace UseCases;

public class GeometriesProvider<TGeometry> : IGeometriesProvider<TGeometry> where TGeometry : Geometry
{
    private readonly IGeometryByScreenService<TGeometry> _geometryByScreenService;

    public GeometriesProvider(IGeometryByScreenService<TGeometry> geometryByScreenService)
    {
        _geometryByScreenService = geometryByScreenService;
    }

    public Task<IEnumerable<AreaIntersectionDto<TGeometry>>> GetGeometryByScreen(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken)
    {
        Polygon polygon = PolygonUtils.FromRectangle(pointLeftBottom, pointRightTop);
        
        return _geometryByScreenService.GetGeometryByScreen(polygon, cancellationToken);
    }
}