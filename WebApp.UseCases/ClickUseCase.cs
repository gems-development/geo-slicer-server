using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Interfaces;

namespace WebApp.UseCases;

public class ClickUseCase<TInfo> : IClickUseCase<TInfo>
{
    private readonly IInfoRepository<TInfo> _repository;

    public ClickUseCase(IInfoRepository<TInfo> repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        return _repository.GetInfoByClick(point, cancellationToken);
    }
}