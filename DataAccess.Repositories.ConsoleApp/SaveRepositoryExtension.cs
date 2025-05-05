using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace DataAccess.Repositories.ConsoleApp;

public static class SaveRepositoryExtension
{
    public static void AddSaveRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRepository<
                GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>, int>,
            SaveRepository>();
    }
}