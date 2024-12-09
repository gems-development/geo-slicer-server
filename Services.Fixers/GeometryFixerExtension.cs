using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.Fixers.Interfaces;

namespace Services.Fixers
{
    public static class GeometryFixerExtension
    {
        public static void AddGeometryFixer(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFixerFactory<Polygon>, FixerFactory>();
            serviceCollection.AddTransient<IGeometryFixer<Polygon>, GeometryFixer<Polygon>>();
        }
    }
}