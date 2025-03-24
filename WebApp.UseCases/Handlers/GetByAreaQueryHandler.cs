using DataAccess.Interfaces;
using MediatR;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Requests;
using WebApp.UseCases.Services;

namespace WebApp.UseCases.Handlers;

public class GetByAreaQueryHandler : IRequestHandler<GetByAreaQuery, MultiPolygon>
{
    private readonly AreaService<MultiPolygon> _service;

    public GetByAreaQueryHandler(AreaService<MultiPolygon> service)
    {
        _service = service;
    }

    public Task<MultiPolygon> Handle(GetByAreaQuery request, CancellationToken cancellationToken)
    {
        return _service.GetGeometryByRectangle(request.PointLeftBottom, request.PointRightTop);
    }
}