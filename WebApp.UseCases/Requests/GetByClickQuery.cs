using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Requests;

public record GetByClickQuery(Point Point) : IRequest<IEnumerable<ClickInfoDto<string>>>;