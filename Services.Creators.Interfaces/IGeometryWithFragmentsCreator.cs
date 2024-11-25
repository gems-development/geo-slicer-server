using DomainModels;
namespace Services.Creators.Interfaces
{
    public interface IGeometryWithFragmentsCreator<TGeometry, TSliceType>
    {
        GeometryWithFragments<TGeometry, TSliceType> ToGeometryWithFragments(TGeometry geometry);
    }
}