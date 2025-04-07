using System.Collections.Generic;
using DomainModels;
using Services.GeometryCreators.Interfaces;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometryCreators
{
    public class GeometryWithFragmentsCreator<TGeometry, TSliceType> : IGeometryWithFragmentsCreator<TGeometry, TSliceType>
    {
        private IGeometrySlicer<TGeometry, TSliceType> _geometrySlicer;

        public GeometryWithFragmentsCreator(
            IGeometrySlicer<TGeometry, TSliceType> geometrySlicer)
        {
            _geometrySlicer = geometrySlicer;
        }

        //todo добавить обработку ошибок
        public GeometryWithFragments<TGeometry, TSliceType> ToGeometryWithFragments(TGeometry geometry)
        {
            IEnumerable<TSliceType> fragments = _geometrySlicer.Slice(geometry);
            GeometryWithFragments<TGeometry, TSliceType> geometryWithFragments =
                new GeometryWithFragments<TGeometry, TSliceType>(geometry, fragments);
            return geometryWithFragments;
        }
    }
}