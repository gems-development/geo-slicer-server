using UseCases.Interfaces;
using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using Mediatr.Queries;

namespace Mediatr.Handlers;

public class GetByAreaQueryHandler : IRequestHandler<GetByAreaQuery, IEnumerable<AreaIntersectionDto<Geometry>>>
{
    private readonly IGeometriesProvider _useCase;

    public GetByAreaQueryHandler(IGeometriesProvider useCase)
    {
        _useCase = useCase;
    }

    public Task<IEnumerable<AreaIntersectionDto<Geometry>>> Handle(GetByAreaQuery request, CancellationToken cancellationToken)
    {
        return _useCase.GetGeometryByScreen(request.PointLeftBottom.CreatePoint(), request.PointRightTop.CreatePoint(), cancellationToken);
    }
}