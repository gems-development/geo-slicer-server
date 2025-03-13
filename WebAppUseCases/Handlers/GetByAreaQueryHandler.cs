using DataAccess.Interfaces;
using MediatR;
using NetTopologySuite.Geometries;
using WebAppUseCases.Requests;
using WebAppUseCases.Services;

namespace WebAppUseCases.Handlers;

public class GetByAreaQueryHandler : IRequestHandler<GetByAreaQuery, IEnumerable<MultiPolygon>>
{
    private readonly GeometryDbContext _context;
    private readonly ClickService _service;

    public GetByAreaQueryHandler(GeometryDbContext context, ClickService service)
    {
        _context = context;
        _service = service;
    }

    public Task<IEnumerable<MultiPolygon>> Handle(GetByAreaQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}