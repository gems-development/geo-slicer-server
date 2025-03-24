using MediatR;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Requests;
using WebApp.UseCases.Services.Interfaces;

namespace WebApp.UseCases.Handlers;

public class GetByAreaQueryHandler : IRequestHandler<GetByAreaQuery, Geometry>
{
    private readonly IAreaService<Geometry> _service;

    public GetByAreaQueryHandler(IAreaService<Geometry> service)
    {
        _service = service;
    }

    public Task<Geometry> Handle(GetByAreaQuery request, CancellationToken cancellationToken)
    {
        return _service.GetGeometryByRectangle(request.PointLeftBottom, request.PointRightTop, cancellationToken);
    }
}