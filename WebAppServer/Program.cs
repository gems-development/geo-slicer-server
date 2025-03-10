
using System.Reflection;
using DataAccess.Interfaces;
using DataAccess.PostgreSql;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => 
    options.AddPolicy(
        "default", 
        policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    ));
builder.Services.AddTransient<GeometryDbContext>(_ => new PostgreApplicationContext(connectionString!));
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