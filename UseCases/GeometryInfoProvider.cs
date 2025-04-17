using UseCases.Interfaces;
using NetTopologySuite.Geometries;
using Services.GeometryProviders.Interfaces;
using WebApp.Dto.Responses;

namespace UseCases;

public class GeometryInfoProvider<TId> : IGeometryInfoProvider<TId>
{
    private readonly IGeometryInfoService<TId> _service;

    public GeometryInfoProvider(IGeometryInfoService<TId> service)
    {
        _service = service;
    }

    public Task<IEnumerable<ClickInfoDto<TId>>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(point, cancellationToken);
    }
}