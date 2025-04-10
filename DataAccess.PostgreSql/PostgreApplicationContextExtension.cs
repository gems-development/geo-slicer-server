using DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.PostgreSql;

public static class PostgreApplicationContextExtension
{
    public static void AddGeometryDbContext(
        this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddTransient<GeometryDbContext>(provider => new PostgreApplicationContext(connectionString));
    }
}