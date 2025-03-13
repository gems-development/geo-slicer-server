using NetTopologySuite.Geometries;

namespace WebAppUseCases.Repositories.Interfaces;

public interface IInfoRepository<TInfo>
{
    Task<IEnumerable<TInfo>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}