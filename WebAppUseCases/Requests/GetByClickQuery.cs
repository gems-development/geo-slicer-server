using MediatR;
using NetTopologySuite.Geometries;

namespace WebAppUseCases.Requests;

public record GetByClickQuery(Point Point) : IRequest<IEnumerable<long>>;