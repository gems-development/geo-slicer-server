using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using NetTopologySuite.Geometries;
using WebApp.Mediatr.Handlers;
using WebApp.Services;
using WebApp.Services.Interfaces;
using WebApp.UseCases;
using WebApp.UseCases.Interfaces;

namespace WebApp.Server;

public static class DependencyContainerFiller
{
    public static void Fill(ref WebApplicationBuilder builder, IConfigurationRoot configuration)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetByAreaQueryHandler)));
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddSingleton<GeometryDbContext>(_ => new PostgreApplicationContext(connectionString!));
        builder.Services.AddSingleton<IGeometryInfoService<string>, GeometryInfoService>();
        builder.Services.AddSingleton<IGeometryByScreenService<Geometry>, GeometryByScreenService>();
        builder.Services.AddSingleton<IRectangleToPolygonService, RectangleToPolygonService>();
        builder.Services.AddScoped<IClickUseCase<string>, ClickUseCase<string>>();
        builder.Services.AddScoped<IAreaUseCase<Geometry>, AreaUseCase<Geometry>>();
    }
}