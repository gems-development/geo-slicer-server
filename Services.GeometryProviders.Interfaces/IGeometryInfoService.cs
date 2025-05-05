using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace Services.GeometryProviders.Interfaces;

public interface IGeometryInfoService<TId>
{
    Task<IEnumerable<ClickInfoDto<TId>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}