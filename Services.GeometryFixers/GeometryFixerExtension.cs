using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;

namespace Services.GeometryFixers;

public static class GeometryFixerExtension
{
    public static void AddGeometryFixer(
        this IServiceCollection serviceCollection, double epsilon)
    {
        serviceCollection.AddTransient<ISpecificFixerFactory<Geometry>>(_ => new SpecificFixerFactory(epsilon));
        serviceCollection.AddTransient<IGeometryFixer<Geometry>, GeometryFixer<Geometry>>();
    }
}