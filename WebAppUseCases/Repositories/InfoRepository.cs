using DataAccess.Interfaces;
using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories.Interfaces;

namespace WebAppUseCases.Repositories;

public class InfoRepository<TInfo> : IInfoRepository<TInfo>
{
    private GeometryDbContext _geometryDbContext;

    public InfoRepository(GeometryDbContext geometryDbContext)
    {
        _geometryDbContext = geometryDbContext;
    }

    public async Task<IEnumerable<TInfo>> GetInfoByClick(Point point)
    {
        throw new NotImplementedException();
    }
}