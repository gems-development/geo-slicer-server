using MediatR;
using WebApp.Dto.Responses;
using WebApp.Mediatr.Queries;
using WebApp.UseCases.Interfaces;

namespace WebApp.Mediatr.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<ClickInfoDto<string>>>
{
    private readonly IClickUseCase<string> _useCase;

    public GetByClickQueryHandler(IClickUseCase<string> useCase)
    {
        _useCase = useCase;
    }

    public Task<IEnumerable<ClickInfoDto<string>>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _useCase.GetInfoByClick(request.Point, cancellationToken);
    }
}