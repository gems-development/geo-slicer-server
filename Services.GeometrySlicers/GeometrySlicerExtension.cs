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
        int maximumNumberOfPoints = -1)
    {
        if (slicerType == GeometrySlicerType.OppositeSlicer)
        {
            if (maximumNumberOfPoints <= 0)
                throw new ArgumentException("Maximum number of points must be greater than zero");
            serviceCollection
                .AddTransient<IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>>
                (_ => new GeometryWithFragmentsGeometrySlicer(
                    new OppositesGeometrySlicerAdapter(new OppositeGeometrySlicerFactory(maximumNumberOfPoints)),
                    epsilon));
        }
    }
}