using ConsoleApp.IRepositories;
using ConsoleApp.IServices;
using DomainModels;
using ISlicers;

namespace ConsoleApp.Services
{
    public class GeometryService<TGeometry, TKey, TSliceType> : IGeometryService<TGeometry, TKey>
    {
        private ISaveRepository<GeometryWithFragments<TGeometry, TSliceType>, TKey> _repository;
        
        private ISlicer<TGeometry, TSliceType> _slicer;

        public GeometryService(
            ISaveRepository<GeometryWithFragments<TGeometry, TSliceType>, TKey> repository,
            ISlicer<TGeometry, TSliceType> slicer)
        {
            _repository = repository;
            _slicer = slicer;
        }

        //todo добавить обработку ошибок
        public TKey Save(TGeometry geometry)
        {
            GeometryWithFragments<TGeometry, TSliceType> geometryWithFragments =
                new GeometryWithFragments<TGeometry, TSliceType>(geometry, _slicer);
            return _repository.Save(geometryWithFragments);
        }
    }
}