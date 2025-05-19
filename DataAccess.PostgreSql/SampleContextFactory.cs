using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.PostgreSql;

public class SampleContextFactory : IDesignTimeDbContextFactory<PostgreApplicationContext>
{
    public PostgreApplicationContext CreateDbContext(string[] args)
    {
        // получаем конфигурацию из файла launchSettings.json проекта WebApp.Server
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("Properties/launchSettings.json");
        IConfigurationRoot config = builder.Build();
 
        // получаем строку подключения из файла launchSettings.json проекта WebApp.Server
        string connectionString = config.GetConnectionString("DefaultConnection")!;
        return new PostgreApplicationContext(connectionString);
    }
}