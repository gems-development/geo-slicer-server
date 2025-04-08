using DomainModels;
namespace Services.GeometryCreators.Interfaces
{
    public interface IGeometryWithFragmentsCreator<TGeometry, TSliceType>
    {
        GeometryWithFragments<TGeometry, TSliceType> ToGeometryWithFragments(TGeometry geometry);
    }
}