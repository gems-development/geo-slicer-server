using MediatR;
using WebApp.UseCases.Requests;
using WebApp.UseCases.Services.Interfaces;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<ClickInfoDto<string>>>
{
    private readonly IClickService<string> _service;

    public GetByClickQueryHandler(IClickService<string> service)
    {
        _service = service;
    }

    public Task<IEnumerable<ClickInfoDto<string>>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(request.Point, cancellationToken);
    }
}