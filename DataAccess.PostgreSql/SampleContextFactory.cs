using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.PostgreSql
{
    public class SampleContextFactory : IDesignTimeDbContextFactory<PostgreApplicationContext>
    {
        public PostgreApplicationContext CreateDbContext(string[] args)
        {
            // получаем конфигурацию из файла appsettings.json
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();
 
            // получаем строку подключения из файла appsettings.json
            string connectionString = config.GetConnectionString("DefaultConnection")!;
            return new PostgreApplicationContext(connectionString);
        }
    }
}