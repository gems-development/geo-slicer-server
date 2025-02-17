using System.Collections.Generic;
using NetTopologySuite.Geometries;
using Slicers.Interfaces;
using GeoSlicer.DivideAndRuleSlicers.OppositesSlicer;

namespace Slicers
{
    public class OppositesSlicerAdapter : ISlicer<Polygon, Polygon>
    {
        private Slicer _slicer;

        public OppositesSlicerAdapter(Slicer slicer)
        {
            _slicer = slicer;
        }
        
        public IEnumerable<Polygon> Slice(Polygon polygon)
        {
            return _slicer.Slice(polygon);
        }
    }
}