using MediatR;
using WebApp.Dto.Requests;
using WebApp.Dto.Responses;

namespace WebApp.Mediatr.Queries;

public record GetByClickQuery(PointDto Point) : IRequest<IEnumerable<ClickInfoDto<string>>>;