using DomainModels;
using NetTopologySuite.Geometries;

namespace Services.GeometryCreators.Interfaces;

public interface IGeometryWithFragmentsCreator<TGeometry, TSliceType> where TGeometry : Geometry
{
    GeometryWithFragments<TGeometry, TSliceType> ToGeometryWithFragments(TGeometry geometry);
}