using GeoSlicer.DivideAndRuleSlicers.OppositesSlicer;

namespace Services.GeometrySlicers.Interfaces;

public interface IOppositeSlicerFactory
{
    public Slicer GetSlicer();
}