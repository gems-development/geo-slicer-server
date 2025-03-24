using MediatR;
using NetTopologySuite.Geometries;

namespace WebApp.UseCases.Requests;

public record GetByAreaQuery(Point PointLeftBottom, Point PointRightTop) : IRequest<MultiPolygon>;