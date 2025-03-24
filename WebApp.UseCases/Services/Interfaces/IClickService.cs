using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Services.Interfaces;

public interface IClickService<TInfo>
{
    Task<IEnumerable<TInfo>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}