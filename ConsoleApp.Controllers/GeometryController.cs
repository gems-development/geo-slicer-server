using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Services.Interfaces;

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

        //todo обработку ошибок
        public TKey SaveGeometry(TGeometryIn geometry)
        {
            GeometryValidateError geometryValidateError = GeometryValidator.ValidateGeometry(geometry);
            geometry = GeometryFixer.FixGeometry(geometry, geometryValidateError);
            GeometryWithFragments<TGeometryIn, TSliceType> geometryOut =
                GeometryWithFragmentsCreator.ToGeometryWithFragments(geometry);
            return Repository.Save(geometryOut);
        }
    }
}