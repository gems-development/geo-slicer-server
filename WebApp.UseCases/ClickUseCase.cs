using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using WebApp.Services.Interfaces;
using WebApp.UseCases.Interfaces;

namespace WebApp.UseCases;

public class ClickUseCase<TInfo> : IClickUseCase<TInfo>
{
    private readonly IGeometryInfoService<TInfo> _service;

    public ClickUseCase(IGeometryInfoService<TInfo> service)
    {
        _service = service;
    }

    public Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(point, cancellationToken);
    }
}