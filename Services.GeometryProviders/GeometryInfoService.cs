using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Services.GeometryProviders.Interfaces;

namespace Services.GeometryProviders;

public class GeometryInfoService : IGeometryInfoService<string>
{
    private GeometryDbContext _geometryDbContext;

    public GeometryInfoService(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }

    public async Task<IEnumerable<ClickInfoDto<string>>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        var res = await _geometryDbContext.GeometryFragments
            .Where(g => g.Fragment.Intersects(point))
            .Select(g => new ClickInfoDto<string>(g.GeometryOriginal.Layer.Alias, g.GeometryOriginal.Properties, g.GeometryOriginalId.ToString()))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return res;
    }
}