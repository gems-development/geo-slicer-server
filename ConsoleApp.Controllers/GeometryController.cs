using System;
using System.Collections.Generic;
using ConsoleApp.Controllers.Helpers;
using ConsoleApp.Services;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Services.Creators.Interfaces;
using Services.ValidateErrors;

namespace ConsoleApp.Controllers
{
    public class GeometryController<TGeometryIn, TSliceType, TKey>
    {
        private IGeometryWithFragmentsCreator<TGeometryIn, TSliceType> GeometryWithFragmentsCreator{ get; set; }
        private IRepository<GeometryWithFragments<TGeometryIn, TSliceType>, TKey> Repository { get; set; }
        private CorrectionService<TGeometryIn> CorrectionService { get; set; }

        public GeometryController(
            IGeometryWithFragmentsCreator<TGeometryIn, TSliceType> geometryWithFragmentsCreator,
            IRepository<GeometryWithFragments<TGeometryIn, TSliceType>, TKey> repository,
            CorrectionService<TGeometryIn> correctionService)
        {
            GeometryWithFragmentsCreator = geometryWithFragmentsCreator;
            Repository = repository;
            CorrectionService = correctionService;
        }
        
        public TKey SaveGeometry(TGeometryIn geometry, out string validateResult, Dictionary<Parameter, bool> parameters)
        {
            validateResult = "";
            try
            {
                GeometryValidateError[]? geometryValidateErrors = null;
                
                bool validatedGeometry = ValidateGeometry(parameters[Parameter.Validate], ref geometryValidateErrors, geometry, ref validateResult);
                geometry = FixGeometry(parameters[Parameter.Fix], validatedGeometry, geometry, geometryValidateErrors);

                GeometryWithFragments<TGeometryIn, TSliceType> geometryOut =
                    GeometryWithFragmentsCreator.ToGeometryWithFragments(geometry);

                return Repository.Save(geometryOut);
            }
            catch (Exception e)
            { 
                Repository.RollbackTransaction();
                throw new Exception(validateResult + "\n" + e.Message, e);
            }
        }

        private bool ValidateGeometry(
            bool parameter, ref GeometryValidateError[]? geometryValidateErrors, TGeometryIn geometry, ref string result)
        {
            return CorrectionService.ValidateGeometry(parameter, ref geometryValidateErrors, geometry, ref result);
        }

        private TGeometryIn FixGeometry(
            bool parameter, bool validatedGeometry, TGeometryIn geometry, GeometryValidateError[]? geometryValidateErrors)
        {
            return CorrectionService.FixGeometry(parameter, validatedGeometry, geometry, geometryValidateErrors);
        }

        public void StartTransaction()
        {
            Repository.StartTransaction();
        }
        
        public void CommitTransaction()
        {
            Repository.CommitTransaction();
        }
        
        public void RollbackTransaction()
        {
            Repository.RollbackTransaction();
        }
    }
}