using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Repositories.Interfaces;

public interface IInfoRepository<TInfo>
{
    Task<IEnumerable<TInfo>> GetInfoByClick(Point point);
}