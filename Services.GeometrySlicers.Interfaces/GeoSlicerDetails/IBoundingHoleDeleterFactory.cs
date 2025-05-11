using GeoSlicer.HoleDeleters;

namespace Services.GeometrySlicers.Interfaces.GeoSlicerDetails;

public interface IBoundingHoleDeleterFactory
{
    BoundingHoleDeleter GetDeleter();
}