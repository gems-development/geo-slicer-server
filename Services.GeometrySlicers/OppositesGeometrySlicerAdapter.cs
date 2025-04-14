using GeoSlicer.DivideAndRuleSlicers;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;

namespace Services.GeometrySlicers;

public class OppositesGeometrySlicerAdapter : IGeometrySlicer<Polygon, Polygon>
{
    private Slicer _slicer;

    public OppositesGeometrySlicerAdapter(IOppositeSlicerFactory slicerFactory)
    {
        _slicer = slicerFactory.GetSlicer();
    }
        
    public IEnumerable<Polygon> Slice(Polygon polygon)
    {
        return _slicer.Slice(polygon, out ICollection<int> skipped);
    }
}