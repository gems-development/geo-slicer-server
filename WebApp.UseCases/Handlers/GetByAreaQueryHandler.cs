using MediatR;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Requests;
using WebApp.UseCases.Services.Interfaces;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Handlers;

public class GetByAreaQueryHandler : IRequestHandler<GetByAreaQuery, IEnumerable<AreaIntersectionDto<Geometry>>>
{
    private readonly IAreaService<Geometry> _service;

    public GetByAreaQueryHandler(IAreaService<Geometry> service)
    {
        _service = service;
    }

    public Task<IEnumerable<AreaIntersectionDto<Geometry>>> Handle(GetByAreaQuery request, CancellationToken cancellationToken)
    {
        return _service.GetGeometryByRectangle(request.PointLeftBottom, request.PointRightTop, cancellationToken);
    }
}