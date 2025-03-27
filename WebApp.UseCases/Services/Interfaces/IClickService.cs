using NetTopologySuite.Geometries;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Services.Interfaces;

public interface IClickService<TInfo>
{
    Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}