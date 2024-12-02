using System;
using System.Linq;
using ConsoleApp.Controllers.Commands;
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
        
        public TKey SaveGeometry(TGeometryIn geometry, out string validateResult, Command[]? commands = null)
        {
            validateResult = "";
            try
            {
                GeometryValidateError[]? geometryValidateErrors = null;
                
                bool validatedGeometry = ValidateGeometry(commands, ref geometryValidateErrors, geometry, ref validateResult);
                geometry = FixGeometry(commands, validatedGeometry, geometry, geometryValidateErrors);

                GeometryWithFragments<TGeometryIn, TSliceType> geometryOut =
                    GeometryWithFragmentsCreator.ToGeometryWithFragments(geometry);

                return Repository.Save(geometryOut);
            }
            catch
            { 
                Repository.RollbackTransaction();
                throw;
            }
        }

        private bool ValidateGeometry(
            Command[]? commands, ref GeometryValidateError[]? geometryValidateErrors, TGeometryIn geometry, ref string result)
        {
            if (commands != null && commands.Any(a => a == Command.Validate))
            {
                geometryValidateErrors = GeometryValidator.ValidateGeometry(geometry);
                result = result + "Validate errors: " + string.Join(" ", geometryValidateErrors);
                return true;
            }
            return false;
        }

        private TGeometryIn FixGeometry(
            Command[]? commands, bool validatedGeometry, TGeometryIn geometry, GeometryValidateError[]? geometryValidateErrors)
        {
            if (commands != null && commands.Any(a => a == Command.Fix))
            {
                if (!validatedGeometry)
                    throw new ApplicationException("geometry was not validated, but a fix command was sent");
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
    }
}