using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using Services.GeometryCreators;
using Services.GeometryFixers;
using Services.GeometrySlicers;
using Services.GeometryValidators;
using UseCases;

namespace ConsoleApp;

public static class GeometrySaverBuilder
{

    public static ServiceProvider BuildGeometrySaverServiceProvider(
        this IServiceCollection _geometrySaverServiceCollection,
        string connectionString,
        int points,
        double epsilonCoordinateComparator,
        double epsilon,
        GeometrySlicerType type)
    {
        _geometrySaverServiceCollection.AddGeometryDbContext(connectionString);
        _geometrySaverServiceCollection.AddSaveRepository();
        _geometrySaverServiceCollection.AddGeometrySlicers(epsilon, type, points);
        _geometrySaverServiceCollection.AddGeometryFixer(epsilonCoordinateComparator);
        _geometrySaverServiceCollection.AddGeometryValidator(epsilonCoordinateComparator);
        _geometrySaverServiceCollection.AddGeometryWithFragmentsCreator();
        _geometrySaverServiceCollection.AddGeometryCorrector();
        _geometrySaverServiceCollection.AddGeometrySaver();
        return _geometrySaverServiceCollection.BuildServiceProvider();
    }
}