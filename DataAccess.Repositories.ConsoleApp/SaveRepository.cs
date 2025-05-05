using DataAccess.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Entities;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;

namespace DataAccess.Repositories.ConsoleApp;

public class SaveRepository : IRepository<GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>, int>
{
    private readonly GeometryDbContext _dbContext;

    private IDbContextTransaction? _transaction;

    private const string SavePointName = "savePoint";

    public SaveRepository(GeometryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public int Save(GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>> objectToSave, string layerAlias, string properties)
    {
        try
        {
            // Проверяем, существует ли Layer с данным Alias
            var existingLayer =  _dbContext.Layers
                .FirstOrDefault(l => l.Alias.Equals(layerAlias));

            // Если Layer существует, используем его, иначе создаём новый
            var layer = existingLayer ?? new Layer { Alias = layerAlias };
            
            GeometryOriginal geometryOriginal = new GeometryOriginal
            {
                Data = objectToSave.Data,
                Layer = layer,
                Properties = properties
            };
            _dbContext.GeometryOriginals.Add(geometryOriginal);
            
            IEnumerable<GeometryFragment> fragments = objectToSave.GeometryFragments.Select(
                fragmentWithNonRenderingBorder =>
                    new GeometryFragment
                    {
                        Fragment = fragmentWithNonRenderingBorder.Fragment,
                        GeometryOriginal = geometryOriginal,
                        NonRenderingBorder = fragmentWithNonRenderingBorder.NonRenderingBorder
                    });
            _dbContext.GeometryFragments.AddRange(fragments);
            _dbContext.SaveChanges();
            return geometryOriginal.Id;
        }
        catch (Exception ex)
        { 
            RollbackTransaction();
            throw new ApplicationException("Error while saving geometry: " + ex.Message, ex);
        }
    }

    public void StartTransaction()
    {
        _transaction = _dbContext.Database.BeginTransaction();
        _transaction.CreateSavepoint(SavePointName);
    }

    public void CommitTransaction()
    {
        _transaction!.Commit();
        _transaction = null;
    }

    public void RollbackTransaction()
    {
        if (_transaction != null)
        {
            _transaction.RollbackToSavepoint(SavePointName);
            _transaction = null;
        }
    }
}