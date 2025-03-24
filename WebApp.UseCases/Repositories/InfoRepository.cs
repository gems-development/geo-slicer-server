using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Repositories.Interfaces;

namespace WebApp.UseCases.Repositories;

public class InfoRepository : IInfoRepository<string>
{
    private GeometryDbContext _geometryDbContext;

    public InfoRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }

    public async Task<IEnumerable<string>> GetInfoByClick(Point point, CancellationToken cancellationToken)
    {
        var res = await _geometryDbContext.GeometryFragments
            .Where(g => g.Fragment.Intersects(point))
            .Select(g => g.GeometryOriginalId)
            .ToHashSetAsync(cancellationToken: cancellationToken);
        return res.Select(g => g.ToString()).AsEnumerable();
    }
}