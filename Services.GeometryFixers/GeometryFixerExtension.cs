using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;

namespace Services.GeometryFixers
{
    public static class GeometryFixerExtension
    {
        public static void AddGeometryFixer(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IGeometryFixerFactory<Polygon>, GeometryGeometryFixerFactory>();
            serviceCollection.AddTransient<IGeometryFixer<Polygon>, GeometryFixer<Polygon>>();
        }
    }
}