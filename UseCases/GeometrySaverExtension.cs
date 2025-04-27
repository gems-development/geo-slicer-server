using UseCases.Interfaces;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace UseCases;

public static class GeometrySaverExtension
{
    public static void AddGeometrySaver(
        this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IGeometrySaver<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>, int>,
                GeometrySaver<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>, int>>();
    }
}