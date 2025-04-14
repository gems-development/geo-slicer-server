using Microsoft.Extensions.DependencyInjection;
using UseCases.Interfaces;

namespace UseCases;

public static class GeometryInfoProviderExtension
{
    public static void AddGeometryInfoProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGeometryInfoProvider<int>, GeometryInfoProvider<int>>();
    }
}