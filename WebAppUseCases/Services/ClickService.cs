using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;
using WebAppUseCases.Services.Interfaces;

namespace WebAppUseCases.Services;

public class ClickService<TInfo> : IClickService<TInfo>
{
    private readonly IInfoRepository<TInfo> _repository;

    public ClickService(IInfoRepository<TInfo> repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<TInfo>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        return _repository.GetInfoByClick(point, cancellationToken);
    }
}