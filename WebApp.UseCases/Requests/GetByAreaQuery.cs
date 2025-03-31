using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Utils.Dto.Responses;

namespace WebApp.UseCases.Requests;

public record GetByAreaQuery(Point PointLeftBottom, Point PointRightTop) : IRequest<IEnumerable<AreaIntersectionDto<Geometry>>>;