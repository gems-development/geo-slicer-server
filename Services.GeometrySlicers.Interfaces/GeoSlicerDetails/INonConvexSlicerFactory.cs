using GeoSlicer.NonConvexSlicer;

namespace Services.GeometrySlicers.Interfaces.GeoSlicerDetails;

public interface INonConvexSlicerFactory
{
    Slicer GetSlicer();
}