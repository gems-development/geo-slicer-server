using MediatR;
using WebAppUseCases.Requests;
using WebAppUseCases.Services.Interfaces;

namespace WebAppUseCases.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<string>>
{
    private readonly IClickService<string> _service;

    public GetByClickQueryHandler(IClickService<string> service)
    {
        _service = service;
    }

    public Task<IEnumerable<string>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(request.Point, cancellationToken);
    }
}