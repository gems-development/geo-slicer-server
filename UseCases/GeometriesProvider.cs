using UseCases.Interfaces;
using NetTopologySuite.Geometries;
using Services.GeometryProviders.Interfaces;
using Services.ScaleCalculator.Interfaces;
using Utils;
using WebApp.Dto.Responses;

namespace UseCases;

public class GeometriesProvider : IGeometriesProvider
{
    private readonly IGeometryByScreenService _geometryByScreenService;
    private readonly IToleranceCalculator _toleranceCalculator;

    public GeometriesProvider(IGeometryByScreenService geometryByScreenService, IToleranceCalculator toleranceCalculator)
    {
        _geometryByScreenService = geometryByScreenService;
        _toleranceCalculator = toleranceCalculator;
    }

    public Task<IEnumerable<AreaIntersectionDto<TGeometry>>> GetGeometryByScreen(Point pointLeftBottom, Point pointRightTop, CancellationToken cancellationToken)
    {
        Polygon polygon = PolygonUtils.FromRectangle(pointLeftBottom, pointRightTop);
        double tolerance = _toleranceCalculator.CalculateTolerance(pointLeftBottom, pointRightTop);
        return _geometryByScreenService.GetGeometryByScreen(polygon, tolerance, cancellationToken);
    }
}