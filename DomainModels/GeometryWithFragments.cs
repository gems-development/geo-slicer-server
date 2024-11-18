using System.Collections.Generic;

namespace DomainModels
{
    public class GeometryWithFragments<TGeometry, TSliceType>
    {
        public TGeometry Data { get; private set; }
        public IEnumerable<TSliceType> GeometryFragments { get; private set; }
        
        public GeometryWithFragments(TGeometry data, IEnumerable<TSliceType> geometryFragments)
        {
            Data = data;
            GeometryFragments = geometryFragments;
        }
    }
}