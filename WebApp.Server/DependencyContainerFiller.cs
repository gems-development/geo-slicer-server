using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using NetTopologySuite.Geometries;
using WebApp.UseCases.Handlers;
using WebApp.UseCases.Repositories;
using WebApp.UseCases.Repositories.Interfaces;
using WebApp.UseCases.Services;
using WebApp.UseCases.Services.Interfaces;

namespace WebApp.Server;

public static class DependencyContainerFiller
{
    public static void Fill(ref WebApplicationBuilder builder, IConfigurationRoot configuration)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetByAreaQueryHandler)));
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddSingleton<GeometryDbContext>(_ => new PostgreApplicationContext(connectionString!));
        builder.Services.AddSingleton<IInfoRepository<string>, InfoRepository>();
        builder.Services.AddSingleton<IGeometryRepository<Geometry>, GeometryRepository>();
        builder.Services.AddScoped<IClickService<string>, ClickService<string>>();
        builder.Services.AddScoped<IAreaService<Geometry>, AreaService<Geometry>>();
    }
}