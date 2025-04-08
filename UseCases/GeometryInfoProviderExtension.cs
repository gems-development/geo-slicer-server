using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using UseCases.Interfaces;

namespace UseCases;

public static class GeometryInfoProviderExtension
{
    public static void AddGeometryInfoProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGeometryInfoProvider<string>, GeometryInfoProvider<string>>();
    }
}