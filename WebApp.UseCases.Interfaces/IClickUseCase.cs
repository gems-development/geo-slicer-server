using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace WebApp.UseCases.Interfaces;

public interface IClickUseCase<TInfo>
{
    Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}