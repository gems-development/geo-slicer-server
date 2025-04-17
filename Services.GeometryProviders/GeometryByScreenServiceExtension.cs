using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryProviders.Interfaces;

namespace Services.GeometryProviders;

public static class GeometryByScreenServiceExtension
{
    public static void AddGeometryByScreenService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGeometryByScreenService, GeometryByScreenService>();
    }
}