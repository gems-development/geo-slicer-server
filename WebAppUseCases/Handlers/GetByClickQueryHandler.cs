using DataAccess.Interfaces;
using MediatR;
using NetTopologySuite.Geometries;
using WebAppUseCases.Requests;
using WebAppUseCases.Services;

namespace WebAppUseCases.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<string>>
{
    private readonly ClickService<string> _service;

    public GetByClickQueryHandler(ClickService<string> service)
    {
        _service = service;
    }

    public Task<IEnumerable<string>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(request.Point, cancellationToken);
    }
}