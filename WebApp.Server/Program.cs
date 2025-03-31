using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;
using WebApp.Server;
using WebApp.Utils.ExceptionHandlers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

builder.Services.AddExceptionHandler<OperationCanceledExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddPolicy(
        "default",
        policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    ));

DependencyContainerFiller.Fill(ref builder, configuration);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog(Log.Logger);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");

app.MapControllers();

app.Run();