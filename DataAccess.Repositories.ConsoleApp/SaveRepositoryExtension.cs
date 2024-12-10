using System;
using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace DataAccess.Repositories.ConsoleApp
{
    public static class SaveRepositoryExtension
    {
        public static void AddSaveRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<GeometryDbContext>(provider => provider.GetRequiredService<GeometryDbContext>());
            serviceCollection.AddSingleton<IRepository<
                    GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>, int>,
                SaveRepository>();
        }
    }
}