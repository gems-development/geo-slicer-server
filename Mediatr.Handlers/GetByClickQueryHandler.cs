using UseCases.Interfaces;
using MediatR;
using WebApp.Dto.Responses;
using Mediatr.Queries;

namespace Mediatr.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<ClickInfoDto<int>>>
{
    private readonly IGeometryInfoProvider<int> _useCase;

    public GetByClickQueryHandler(IGeometryInfoProvider<int> useCase)
    {
        _useCase = useCase;
    }

    public Task<IEnumerable<ClickInfoDto<int>>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _useCase.GetInfoByClick(request.Point.CreatePoint(), cancellationToken);
    }
}