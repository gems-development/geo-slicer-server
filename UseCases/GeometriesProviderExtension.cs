using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using UseCases.Interfaces;

namespace UseCases;

public static class GeometriesProviderExtension
{
    public static void AddGeometriesProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGeometriesProvider<Geometry>, GeometriesProvider<Geometry>>();
    }
}