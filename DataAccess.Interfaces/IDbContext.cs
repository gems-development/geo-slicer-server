using System;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Interfaces
{
    public interface IDbContext : IDisposable
    {
        DbSet<GeometryOriginal> GeometryOriginals { get; set; }
        DbSet<GeometryFragment> GeometryFragments { get; set; }
    }
    
}