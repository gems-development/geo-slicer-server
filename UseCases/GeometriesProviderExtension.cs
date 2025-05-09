using Microsoft.Extensions.DependencyInjection;
using Services.ScaleCalculator;
using Services.ScaleCalculator.Interfaces;
using UseCases.Interfaces;

namespace UseCases;

public static class GeometriesProviderExtension
{
    public static void AddGeometriesProvider(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGeometriesProvider, GeometriesProvider>();
        serviceCollection.AddScoped<IToleranceCalculator>(provider => new LinearToleranceCalculator(5E-4));
    }
}