using UseCases;
using UseCases.Interfaces;
using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using NetTopologySuite.Geometries;
using Mediatr.Handlers;
using Services.GeometryProviders;
using Services.GeometryProviders.Interfaces;

namespace WebApp.Server;

public static class DependencyContainerFiller
{
    public static void Fill(ref WebApplicationBuilder builder, IConfigurationRoot configuration)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetByAreaQueryHandler)));
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddScoped<GeometryDbContext>(_ => new PostgreApplicationContext(connectionString!));
        builder.Services.AddScoped<IGeometryInfoService<string>, GeometryInfoService>();
        builder.Services.AddScoped<IGeometryByScreenService<Geometry>, GeometryByScreenService>();
        builder.Services.AddScoped<IGeometryInfoProvider<string>, GeometryInfoProvider<string>>();
        builder.Services.AddScoped<IGeometriesProvider<Geometry>, GeometriesProvider<Geometry>>();
    }
}