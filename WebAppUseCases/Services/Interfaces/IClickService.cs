using NetTopologySuite.Geometries;

namespace WebAppUseCases.Services.Interfaces;

public interface IClickService<TInfo>
{
    Task<IEnumerable<TInfo>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}