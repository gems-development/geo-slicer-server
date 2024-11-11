using System.Collections.Generic;
using ISlicers;
using NetTopologySuite.Geometries;

namespace DomainModels
{
    public class GeometryWithFragments<TK, TV>
    {
        public TK Data { get; private set; }
        public IEnumerable<TV> GeometryFragments { get; private set; }
        
        public GeometryWithFragments(TK data, IEnumerable<TV> geometryFragments)
        {
            Data = data;
            GeometryFragments = geometryFragments;
        }
    }
}