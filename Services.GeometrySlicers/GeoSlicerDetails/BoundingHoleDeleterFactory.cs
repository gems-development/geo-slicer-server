using GeoSlicer.HoleDeleters;
using Services.GeometrySlicers.Interfaces.GeoSlicerDetails;

namespace Services.GeometrySlicers.GeoSlicerDetails;

public class BoundingHoleDeleterFactory : IBoundingHoleDeleterFactory
{
    public BoundingHoleDeleter GetDeleter()
    {
        return new BoundingHoleDeleter(1e-15);
    }
}