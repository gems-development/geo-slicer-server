using GeoSlicer.HoleDeleters;
using GeoSlicer.NonConvexSlicer;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;
using Services.GeometrySlicers.Interfaces.GeoSlicerDetails;

namespace Services.GeometrySlicers;

public class NonConvexSlicerAdapter : IGeometrySlicer<Polygon, Polygon>
{
    private readonly Slicer _slicer;

    private readonly BoundingHoleDeleter _deleter;

    public NonConvexSlicerAdapter(INonConvexSlicerFactory slicerFactory, IBoundingHoleDeleterFactory deleterFactory)
    {
        _slicer = slicerFactory.GetSlicer();
        _deleter = deleterFactory.GetDeleter();
    }
            
    public IEnumerable<Polygon> Slice(Polygon polygon)
    {
        Polygon polygonWithoutHoles = _deleter.DeleteHoles(polygon);
        return _slicer.Slice(polygonWithoutHoles.Shell).Select(a => new Polygon(a)).ToArray();
    }
    
}