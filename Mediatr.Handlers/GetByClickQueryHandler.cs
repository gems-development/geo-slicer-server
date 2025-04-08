using UseCases.Interfaces;
using MediatR;
using WebApp.Dto.Responses;
using Mediatr.Queries;

namespace Mediatr.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<ClickInfoDto<string>>>
{
    private readonly IGeometryInfoProvider<string> _useCase;

    public GetByClickQueryHandler(IGeometryInfoProvider<string> useCase)
    {
        _useCase = useCase;
    }

    public Task<IEnumerable<ClickInfoDto<string>>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _useCase.GetInfoByClick(request.Point.CreatePoint(), cancellationToken);
    }
}