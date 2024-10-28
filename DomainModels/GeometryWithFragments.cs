using System.Collections.Generic;
using ISlicers;
using NetTopologySuite.Geometries;

namespace DomainModels
{
    public class GeometryWithFragments<TK, TV>
    {
        public TK Data { get; private set; }
        public IEnumerable<TV> GeometryFragments { get; private set; }
        
        public GeometryWithFragments(TK data, ISlicer <TK, TV> slicer)
        {
            Data = data;
            GeometryFragments = slicer.Slice(data);
        }
    }
}