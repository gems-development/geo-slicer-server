using MediatR;
using WebApp.Dto.Requests;
using WebApp.Dto.Responses;

namespace Mediatr.Queries;

public record GetByClickQuery(PointDto Point) : IRequest<IEnumerable<ClickInfoDto<int>>>;