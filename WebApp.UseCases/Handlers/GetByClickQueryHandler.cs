using DataAccess.Interfaces;
using MediatR;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Requests;
using WebApp.UseCases.Services;

namespace WebApp.UseCases.Handlers;

public class GetByClickQueryHandler : IRequestHandler<GetByClickQuery, IEnumerable<string>>
{
    private readonly ClickService<string> _service;

    public GetByClickQueryHandler(ClickService<string> service)
    {
        _service = service;
    }

    public Task<IEnumerable<string>> Handle(GetByClickQuery request, CancellationToken cancellationToken)
    {
        return _service.GetInfoByClick(request.Point);
    }
}