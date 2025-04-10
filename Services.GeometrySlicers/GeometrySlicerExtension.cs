using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometrySlicers;

public static class GeometrySlicerExtension
{
    public static void AddGeometrySlicers(
        this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IGeometrySlicer<Polygon, Polygon>, OppositesGeometrySlicerAdapter>();
            
        serviceCollection
            .AddTransient<
                IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>,
                GeometryWithFragmentsGeometrySlicer>();
    }
}