using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;
using GeoSlicer.DivideAndRuleSlicers.OppositesSlicer;

namespace Services.GeometrySlicers;

public class OppositesGeometrySlicerAdapter : IGeometrySlicer<Polygon, Polygon>
{
    private Slicer _slicer;

    public OppositesGeometrySlicerAdapter(Slicer slicer)
    {
        _slicer = slicer;
    }
        
    public IEnumerable<Polygon> Slice(Polygon polygon)
    {
        return _slicer.Slice(polygon);
    }
}