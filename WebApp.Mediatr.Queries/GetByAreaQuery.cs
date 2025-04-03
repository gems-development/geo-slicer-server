using MediatR;
using NetTopologySuite.Geometries;
using WebApp.Dto.Requests;
using WebApp.Dto.Responses;

namespace WebApp.Mediatr.Queries;

public record GetByAreaQuery(PointDto PointLeftBottom, PointDto PointRightTop) : IRequest<IEnumerable<AreaIntersectionDto<Geometry>>>;