using NetTopologySuite.Geometries;

namespace Services.GeometryFixers.Interfaces;

public interface ISpecificFixer<TGeometry> where TGeometry : Geometry
{
    public TGeometry Fix(TGeometry geometry);
}