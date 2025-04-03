using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace WebApp.Mediatr.Queries;

public record GetByClickQuery(Point Point) : IRequest<IEnumerable<ClickInfoDto<string>>>;