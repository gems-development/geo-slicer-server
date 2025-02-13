using Services.Fixers.Interfaces;
using Services.ValidateErrors;
using Services.Validators.Interfaces;

namespace ConsoleApp.Services
{
    public class CorrectionService<TGeometryIn>
    {
        private IGeometryValidator<TGeometryIn> GeometryValidator { get; set; }
        private IGeometryFixer<TGeometryIn> GeometryFixer { get; set; }
        
        public CorrectionService(
            IGeometryValidator<TGeometryIn> geometryValidator, 
            IGeometryFixer<TGeometryIn> geometryFixer)
        {
            GeometryValidator = geometryValidator;
            GeometryFixer = geometryFixer;
        }
        public bool ValidateGeometry(
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

        public TGeometryIn FixGeometry(
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
    }
}