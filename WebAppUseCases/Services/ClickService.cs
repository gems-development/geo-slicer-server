using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;
using WebAppUseCases.Services.Interfaces;

namespace WebAppUseCases.Services;

public class ClickService<TInfo> : IClickService<TInfo>
{
    private IInfoRepository<TInfo> _repository;

    public ClickService(IInfoRepository<TInfo> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TInfo>> GetInfoByClick(Point point)
    {
        return await _repository.GetInfoByClick(point);
    }
}