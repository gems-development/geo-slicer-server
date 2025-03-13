
using System.Reflection;
using DataAccess.Interfaces;
using DataAccess.PostgreSql;
using NetTopologySuite.Geometries;
using WebAppUseCases.Repositories;
using WebAppUseCases.Repositories.Interfaces;
using WebAppUseCases.Services;
using WebAppUseCases.Services.Interfaces;
using WebAppUtils;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
// var connectionString = configuration.GetConnectionString("DefaultConnection");

//move DC filling to different class
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => 
    options.AddPolicy(
        "default", 
        policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    ));
// builder.Services.AddSingleton<GeometryDbContext>(_ => new PostgreApplicationContext(connectionString!));
// builder.Services.AddSingleton<IInfoRepository<string>, InfoRepository>();
// builder.Services.AddSingleton<IGeometryRepository<Geometry>, GeometryRepository>();
// builder.Services.AddScoped<IClickService<string>, ClickService<string>>();
// builder.Services.AddScoped<IAreaService<Geometry>, AreaService<Geometry>>();
DependencyContainerFiller.Fill(ref builder, configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("default");

app.MapControllers();

app.Run();