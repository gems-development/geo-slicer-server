using MediatR;
using NetTopologySuite.Geometries;

namespace WebAppUseCases.Requests;

public record GetByAreaQuery(Point PointLeftBottom, Point PointRightTop) : IRequest<IEnumerable<MultiPolygon>>;