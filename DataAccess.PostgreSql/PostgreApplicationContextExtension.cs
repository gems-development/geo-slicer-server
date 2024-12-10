using DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace DataAccess.PostgreSql
{
    public static class PostgreApplicationContextExtension
    {
        public static void AddGeometryDbContext(
            this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddSingleton<GeometryDbContext>(provider => new PostgreApplicationContext(connectionString));
        }
    }
}