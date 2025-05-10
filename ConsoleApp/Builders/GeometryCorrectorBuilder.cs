using Microsoft.Extensions.DependencyInjection;
using Services.GeometryFixers;
using Services.GeometryValidators;
using UseCases;

namespace ConsoleApp.Builders;

internal static class GeometryCorrectorBuilder
{
    internal static  ServiceProvider BuildGeometryCorrectorProvider(
        this IServiceCollection geometryCorrectorServiceCollection,
        double epsilonCoordinateComparator)
    {
        geometryCorrectorServiceCollection.AddGeometryFixer(epsilonCoordinateComparator);
        geometryCorrectorServiceCollection.AddGeometryValidator(epsilonCoordinateComparator);
        geometryCorrectorServiceCollection.AddGeometryCorrector();
        return geometryCorrectorServiceCollection.BuildServiceProvider();
    }
}