using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using WebApp.Services.Interfaces;
using WebApp.UseCases.Interfaces;

namespace WebApp.UseCases;

public class AreaUseCase<TGeometry> : IAreaUseCase<TGeometry> where TGeometry : Geometry
{
    private readonly IGeometryByScreenService<TGeometry> _geometryByScreenService;
    
    private readonly IRectangleToPolygonService _rectangleToPolygonService;

    public AreaUseCase(IGeometryByScreenService<TGeometry> geometryByScreenService, IRectangleToPolygonService rectangleToPolygonService)
    {
        _geometryByScreenService = geometryByScreenService;
        _rectangleToPolygonService = rectangleToPolygonService;
    }

    public Task<IEnumerable<AreaIntersectionDto<Geometry>>> GetGeometryByScreen(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken)
    {
        Polygon polygon = _rectangleToPolygonService.CreatePolygon(pointLeftBottom, pointRightTop);
        
        return _geometryByScreenService.GetGeometryByScreen(polygon, cancellationToken);
    }
}