using UseCases;
using DataAccess.PostgreSql;
using Mediatr.Handlers;
using Services.GeometryProviders;

namespace WebApp.Server;

public static class DependencyContainerFiller
{
    public static void Fill(ref WebApplicationBuilder builder, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetByAreaQueryHandler)));
        builder.Services.AddGeometryDbContext(connectionString!);
        builder.Services.AddGeometryInfoService();
        builder.Services.AddGeometryByScreenService();
        builder.Services.AddGeometryInfoProvider();
        builder.Services.AddGeometriesProvider();
    }
}