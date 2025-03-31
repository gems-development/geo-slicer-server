using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Dto.Responses;

namespace WebApp.Mediatr.Queries;

public record GetByAreaQuery(Point PointLeftBottom, Point PointRightTop) : IRequest<IEnumerable<AreaIntersectionDto<Geometry>>>;