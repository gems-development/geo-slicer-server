using System;
using System.Collections.Generic;
using ConsoleApp.Controllers.Parameters;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Services.Creators.Interfaces;
using Services.Fixers.Interfaces;
using Services.ValidateErrors;
using Services.Validators.Interfaces;

namespace ConsoleApp.Controllers
{
    public class GeometryController<TGeometryIn, TSliceType, TKey>
    {
        private IGeometryWithFragmentsCreator<TGeometryIn, TSliceType> GeometryWithFragmentsCreator{ get; set; }
        private IRepository<GeometryWithFragments<TGeometryIn, TSliceType>, TKey> Repository { get; set; }
        private IGeometryValidator<TGeometryIn> GeometryValidator { get; set; }
        private IGeometryFixer<TGeometryIn> GeometryFixer { get; set; }

        public GeometryController(
            IGeometryWithFragmentsCreator<TGeometryIn, TSliceType> geometryWithFragmentsCreator,
            IRepository<GeometryWithFragments<TGeometryIn, TSliceType>, TKey> repository, 
            IGeometryValidator<TGeometryIn> geometryValidator, 
            IGeometryFixer<TGeometryIn> geometryFixer)
        {
            GeometryWithFragmentsCreator = geometryWithFragmentsCreator;
            Repository = repository;
            GeometryValidator = geometryValidator;
            GeometryFixer = geometryFixer;
        }
        
        public TKey SaveGeometry(TGeometryIn geometry, out string validateResult, Dictionary<Parameter,bool> parameters)
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
            if (parameter)
            {
                geometryValidateErrors = GeometryValidator.ValidateGeometry(geometry);
                result = result + "Validate errors: " + string.Join("\n", geometryValidateErrors);
                return true;
            }
            return false;
        }

        private TGeometryIn FixGeometry(
            bool parameter, bool validatedGeometry, TGeometryIn geometry, GeometryValidateError[]? geometryValidateErrors)
        {
            if (parameter)
            {
                if (!validatedGeometry)
                    throw new ApplicationException("geometry was not validated, but a fix parameter was sent");
                geometry = GeometryFixer.FixGeometry(geometry, geometryValidateErrors!);
            }

            return geometry;
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