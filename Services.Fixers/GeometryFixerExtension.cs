using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.Fixers.Interfaces;

namespace Services.Fixers
{
    public static class GeometryFixerExtension
    {
        public static void AddGeometryFixer(
            this IServiceCollection serviceCollection, EpsilonCoordinateComparator epsilonCoordinateComparator)
        {
            serviceCollection.AddTransient<IFixerFactory<Polygon>>(provider => new FixerFactory(epsilonCoordinateComparator));
            serviceCollection.AddTransient<IGeometryFixer<Polygon>, GeometryFixer<Polygon>>();
        }
    }
}