using GeoSlicer.DivideAndRuleSlicers;

namespace Services.GeometrySlicers.Interfaces;

public interface IOppositeSlicerFactory
{
    public Slicer GetSlicer();
}