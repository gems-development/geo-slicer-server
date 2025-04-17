using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometrySlicers;

public static class GeometrySlicerExtension
{
    public static void AddGeometrySlicers(
        this IServiceCollection serviceCollection, double epsilon,
        int maximumNumberOfPoints = -1)
    {
        serviceCollection.AddTransient<IOppositeSlicerFactory>(_ =>
            new OppositeGeometrySlicerFactory(maximumNumberOfPoints));

        serviceCollection.AddTransient<IGeometrySlicer<Polygon, Polygon>, OppositesGeometrySlicerAdapter>();

        serviceCollection
            .AddTransient<IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>>
                (provider => new GeometryWithFragmentsGeometrySlicer(provider.GetService<IGeometrySlicer<Polygon, Polygon>>()!, epsilon));
    }
}