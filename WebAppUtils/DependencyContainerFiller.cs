using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories;
using WebAppUseCases.Repositories.Interfaces;
using WebAppUseCases.Services;
using WebAppUseCases.Services.Interfaces;

namespace WebAppUtils;

public static class DependencyContainerFiller
{
    public static void Fill(ref WebApplicationBuilder builder, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddSingleton<GeometryDbContext>(_ => new PostgreApplicationContext(connectionString!));
        builder.Services.AddSingleton<IInfoRepository<string>, InfoRepository>();
        builder.Services.AddSingleton<IGeometryRepository<Geometry>, GeometryRepository>();
        builder.Services.AddScoped<IClickService<string>, ClickService<string>>();
        builder.Services.AddScoped<IAreaService<Geometry>, AreaService<Geometry>>();
    }
}