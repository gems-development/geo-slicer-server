using System;
using IGeometryFixers;
using IGeometryValidators;
using ConsoleApp.IRepositories;
using GeometryValidateErrors;

namespace ConsoleApp.Controllers
{
    public class GeometryController<TGeometryIn, TGeometryOut, TKey>
    {
        private Func<TGeometryIn, TGeometryOut> GeometryService { get; set; }
        private ISaveRepository<TGeometryOut, TKey> SaveRepository { get; set; }
        private IGeometryValidator<TGeometryIn> GeometryValidator { get; set; }
        private IGeometryFixer<TGeometryIn> GeometryFixer { get; set; }

        public GeometryController(
            Func<TGeometryIn, TGeometryOut> geometryService,
            ISaveRepository<TGeometryOut, TKey> saveRepository, 
            IGeometryValidator<TGeometryIn> geometryValidator, 
            IGeometryFixer<TGeometryIn> geometryFixer)
        {
            GeometryService = geometryService;
            SaveRepository = saveRepository;
            GeometryValidator = geometryValidator;
            GeometryFixer = geometryFixer;
        }

        //todo обработку ошибок
        public TKey SaveGeometry(TGeometryIn geometry)
        {
            GeometryValidateError geometryValidateError = GeometryValidator.ValidateGeometry(geometry);
            geometry = GeometryFixer.FixGeometry(geometry, geometryValidateError);
            TGeometryOut geometryOut = GeometryService(geometry);
            return SaveRepository.Save(geometryOut);
        }
    }
}