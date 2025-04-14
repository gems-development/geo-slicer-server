using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;

namespace Services.GeometryFixers;

public static class GeometryFixerExtension
{
    public static void AddGeometryFixer(
        this IServiceCollection serviceCollection, double epsilon)
    {
        serviceCollection.AddTransient<IGeometryFixerFactory<Polygon>>(_ => new GeometryFixerFactory(epsilon));
        serviceCollection.AddTransient<IGeometryFixer<Polygon>, GeometryFixer<Polygon>>();
    }
}