using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.PostgreSql
{
    public sealed class PostgreApplicationContext : DbContext, IDbContext
    {
        public DbSet<GeometryOriginal> GeometryOriginals { get; set; }
        public DbSet<GeometryFragment> GeometryFragments { get; set; }

        public PostgreApplicationContext(DbContextOptions<PostgreApplicationContext> options)
            : base(options) {}
        
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin",
                o => o.UseNetTopologySuite());
        }*/
    }
}