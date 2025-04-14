using Microsoft.Extensions.DependencyInjection;
using Services.GeometryProviders.Interfaces;

namespace Services.GeometryProviders;

public static class GeometryInfoServiceExtension
{
    public static void AddGeometryInfoService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGeometryInfoService<int>, GeometryInfoService>();
    }
}