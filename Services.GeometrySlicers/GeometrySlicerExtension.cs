using DomainModels;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometrySlicers;

public static class GeometrySlicerExtension
{
    public static void AddGeometrySlicers(
        this IServiceCollection serviceCollection, double epsilon,
        GeometrySlicerType slicerType,
        int? maximumNumberOfPoints)
    {
        if (slicerType == GeometrySlicerType.OppositeSlicer)
        {
            if (maximumNumberOfPoints!.Value <= 0)
                throw new ArgumentException("Maximum number of points must be greater than zero");
            serviceCollection
                .AddTransient<IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>>
                (_ => new GeometryWithFragmentsGeometrySlicer(
                    new OppositesGeometrySlicerAdapter(new OppositeGeometrySlicerFactory(maximumNumberOfPoints.Value)),
                    epsilon));
        }
        else if (slicerType == GeometrySlicerType.NonConvexSlicer)
        {
            throw new NotSupportedException("NonConvexSlicers are not supported");
        }
    }
}