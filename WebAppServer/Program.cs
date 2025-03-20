using WebAppUtils;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

//add exception handler
//OperationCanceledException

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => 
    options.AddPolicy(
        "default", 
        policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    ));

DependencyContainerFiller.Fill(ref builder, configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");

app.MapControllers();

app.Run();