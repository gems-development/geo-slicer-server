using UseCases.Interfaces;
using NetTopologySuite.Geometries;
using Services.GeometryProviders.Interfaces;
using WebApp.Dto.Responses;

namespace UseCases;

public class GeometryInfoProvider<TInfo> : IGeometryInfoProvider<TInfo>
{
    private readonly IGeometryInfoService<TInfo> _service;

    public GeometryInfoProvider(IGeometryInfoService<TInfo> service)
    {
        _service = service;
    }

    public Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(point, cancellationToken);
    }
}