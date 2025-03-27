using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Services.Interfaces;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Services;

public class ClickService<TInfo> : IClickService<TInfo>
{
    private readonly IInfoRepository<TInfo> _repository;

    public ClickService(IInfoRepository<TInfo> repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        return _repository.GetInfoByClick(point, cancellationToken);
    }
}