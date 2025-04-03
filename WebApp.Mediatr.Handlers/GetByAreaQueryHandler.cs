using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;
using WebApp.Mediatr.Queries;
using WebApp.UseCases.Interfaces;

namespace WebApp.Mediatr.Handlers;

public class GetByAreaQueryHandler : IRequestHandler<GetByAreaQuery, IEnumerable<AreaIntersectionDto<Geometry>>>
{
    private readonly IAreaUseCase<Geometry> _useCase;

    public GetByAreaQueryHandler(IAreaUseCase<Geometry> useCase)
    {
        _useCase = useCase;
    }

    public Task<IEnumerable<AreaIntersectionDto<Geometry>>> Handle(GetByAreaQuery request, CancellationToken cancellationToken)
    {
        return _useCase.GetGeometryByScreen(request.PointLeftBottom.CreatePoint(), request.PointRightTop.CreatePoint(), cancellationToken);
    }
}