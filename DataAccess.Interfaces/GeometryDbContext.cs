using System;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Interfaces
{
    public abstract class GeometryDbContext : DbContext
    {
        public abstract DbSet<GeometryOriginal> GeometryOriginals { get; set; }
        public abstract DbSet<GeometryFragment> GeometryFragments { get; set; }
        public abstract DbSet<Layer> Layers { get; set; }
    }
    
}