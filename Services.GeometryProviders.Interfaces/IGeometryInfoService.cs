using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace Services.GeometryProviders.Interfaces;

public interface IGeometryInfoService<TInfo>
{
    Task<IEnumerable<ClickInfoDto<TInfo>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}