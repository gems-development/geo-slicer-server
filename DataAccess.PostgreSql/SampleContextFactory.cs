using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.PostgreSql
{
    public class SampleContextFactory : IDesignTimeDbContextFactory<PostgreApplicationContext>
    {
        public PostgreApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PostgreApplicationContext>();
 
            // получаем конфигурацию из файла appsettings.json
            ConfigurationBuilder builder = new ConfigurationBuilder();
            if (args.Length == 0)
                builder.SetBasePath(Directory.GetCurrentDirectory());
            else
                builder.SetBasePath(args[0]);
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();
 
            // получаем строку подключения из файла appsettings.json
            string connectionString = config.GetConnectionString("DefaultConnection");
            var options = optionsBuilder.UseNpgsql(
                connectionString,
                o => o.UseNetTopologySuite()).Options;
            return new PostgreApplicationContext(options);
        }
    }
}