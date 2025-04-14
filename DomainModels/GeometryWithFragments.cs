using NetTopologySuite.Geometries;

namespace DomainModels;

public class GeometryWithFragments<TGeometry, TSliceType> where TGeometry : Geometry
{
    public TGeometry Data { get; private set; }
    public IEnumerable<TSliceType> GeometryFragments { get; private set; }
        
    public GeometryWithFragments(TGeometry data, IEnumerable<TSliceType> geometryFragments)
    {
        Data = data;
        GeometryFragments = geometryFragments;
    }
}