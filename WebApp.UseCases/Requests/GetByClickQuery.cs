using MediatR;
using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Requests;

public record GetByClickQuery(Point Point) : IRequest<IEnumerable<string>>;