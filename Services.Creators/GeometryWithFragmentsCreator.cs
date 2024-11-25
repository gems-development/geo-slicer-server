using System.Collections.Generic;
using DomainModels;
using Services.Creators.Interfaces;
using Slicers.Interfaces;

namespace Services.Creators
{
    public class GeometryWithFragmentsCreator<TGeometry, TSliceType> : IGeometryWithFragmentsCreator<TGeometry, TSliceType>
    {
        private ISlicer<TGeometry, TSliceType> _slicer;

        public GeometryWithFragmentsCreator(
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