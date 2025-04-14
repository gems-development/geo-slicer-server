using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace UseCases.Interfaces;

public interface IGeometryInfoProvider<TInfo>
{
    Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}