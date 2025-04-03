using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Entities;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Geometries;

namespace DataAccess.Repositories.ConsoleApp
{
    public class SaveRepository : IRepository<GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>, int>
    {
        private readonly GeometryDbContext _dbContext;

        private IDbContextTransaction? _transaction;

        private const string SavePointName = "savePoint";

        public SaveRepository(GeometryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //todo добавлена заглушка, так как инструмент не учитывает слой и свойства сохраняемой геометрии
        public int Save(GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>> objectToSave)
        {
            try
            {
                GeometryOriginal geometryOriginal = new GeometryOriginal
                {
                    Data = objectToSave.Data,
                    LayerId = 0,
                    Properties = ""
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
}