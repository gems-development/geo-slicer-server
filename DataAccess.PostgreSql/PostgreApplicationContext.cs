using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.PostgreSql;

public sealed class PostgreApplicationContext : GeometryDbContext
{
    public override DbSet<GeometryOriginal> GeometryOriginals { get; set; }
    public override DbSet<GeometryFragment> GeometryFragments { get; set; }
    public override DbSet<Layer> Layers { get; set; }
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
            
        modelBuilder.Entity<Layer>()
            .HasIndex(l => l.Alias)
            .IsUnique();
    }
}