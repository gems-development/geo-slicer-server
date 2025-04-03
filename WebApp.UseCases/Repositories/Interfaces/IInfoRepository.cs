using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace WebApp.UseCases.Repositories.Interfaces;

public interface IInfoRepository<TInfo>
{
    Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}