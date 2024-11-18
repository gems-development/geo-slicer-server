using DomainModels;
namespace Services.Interfaces
{
    public interface IGeometryWithFragmentsCreator<TGeometry, TSliceType>
    {
        GeometryWithFragments<TGeometry, TSliceType> ToGeometryWithFragments(TGeometry geometry);
    }
}