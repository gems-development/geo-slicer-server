using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Services.Interfaces;

namespace WebApp.UseCases.Services;

public class ClickService<TInfo> : IClickService<TInfo>
{
    private IInfoRepository<TInfo> _repository;

    public ClickService(IInfoRepository<TInfo> repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<TInfo>> GetInfoByClick(Point point)
    {
        return _repository.GetInfoByClick(point);
    }
}