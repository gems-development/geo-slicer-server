using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace UseCases.Interfaces;

public interface IGeometryInfoProvider<TId>
{
    Task<IEnumerable<ClickInfoDto<TId>>> GetInfoByClick(Point point, CancellationToken cancellationToken);
}