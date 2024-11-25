using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DataAccess.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Entities;
using GeoSlicer.Utils;
using NetTopologySuite.Geometries;
using Utils;

namespace DataAccess.Repositories.ConsoleApp
{
    public class SaveRepository : IRepository<GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>, int>
    {
        private readonly GeometryDbContext _dbContext;

        private double _epsilon = 1E-14;

        public SaveRepository(GeometryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public int Save(GeometryWithFragments<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>> objectToSave)
        {
            GeometryOriginal geometryOriginal = new GeometryOriginal
            {
                Data = objectToSave.Data
            };
            _dbContext.GeometryOriginals.Add(geometryOriginal);

            IEnumerable<GeometryFragment> fragments = objectToSave.GeometryFragments.Select(fragmentWithNonRenderingBorder =>
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
    }
}