using System.Collections.Generic;
using ConsoleApp.IRepositories;
using DomainModels;
using ISlicers;

namespace ConsoleApp.Services
{
    public class GeometryService<TGeometry, TSliceType>
    {
        private ISlicer<TGeometry, TSliceType> _slicer;

        public GeometryService(
            ISlicer<TGeometry, TSliceType> slicer)
        {
            _slicer = slicer;
        }

        //todo добавить обработку ошибок
        public GeometryWithFragments<TGeometry, TSliceType> ToGeometryWithFragments(TGeometry geometry)
        {
            IEnumerable<TSliceType> fragments = _slicer.Slice(geometry);
            GeometryWithFragments<TGeometry, TSliceType> geometryWithFragments =
                new GeometryWithFragments<TGeometry, TSliceType>(geometry, fragments);
            return geometryWithFragments;
        }
    }
}