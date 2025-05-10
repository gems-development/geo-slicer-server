using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using Services.GeometryCreators;
using Services.GeometryFixers;
using Services.GeometrySlicers;
using Services.GeometryValidators;
using UseCases;

namespace ConsoleApp.Builders;

internal static class GeometrySaverBuilder
{

    internal static ServiceProvider BuildGeometrySaverServiceProvider(
        this IServiceCollection geometrySaverServiceCollection,
        string connectionString,
        int? points,
        double epsilonCoordinateComparator,
        double epsilon,
        GeometrySlicerType type)
    {
        geometrySaverServiceCollection.AddGeometryDbContext(connectionString);
        geometrySaverServiceCollection.AddSaveRepository();
        geometrySaverServiceCollection.AddGeometrySlicers(epsilon, type, points);
        geometrySaverServiceCollection.AddGeometryFixer(epsilonCoordinateComparator);
        geometrySaverServiceCollection.AddGeometryValidator(epsilonCoordinateComparator);
        geometrySaverServiceCollection.AddGeometryWithFragmentsCreator();
        geometrySaverServiceCollection.AddGeometryCorrector();
        geometrySaverServiceCollection.AddGeometrySaver();
        return geometrySaverServiceCollection.BuildServiceProvider();
    }
}