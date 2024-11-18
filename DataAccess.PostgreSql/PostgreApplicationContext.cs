using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.PostgreSql
{
    public sealed class PostgreApplicationContext : DbContext, IDbContext
    {
        public DbSet<GeometryOriginal> GeometryOriginals { get; set; }
        public DbSet<GeometryFragment> GeometryFragments { get; set; }
        private readonly string _connectionString;

        public PostgreApplicationContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies()
                .UseNpgsql(_connectionString, o => o.UseNetTopologySuite());
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeometryOriginal>()
                .HasIndex(g => g.Data).HasMethod("GIST");
            
            modelBuilder.Entity<GeometryFragment>()
                .HasIndex(g => g.Fragment).HasMethod("GIST");
        }
    }
}