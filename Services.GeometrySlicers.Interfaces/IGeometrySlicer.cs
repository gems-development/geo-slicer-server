using NetTopologySuite.Geometries;

namespace Services.GeometrySlicers.Interfaces;

public interface IGeometrySlicer<TGeometry, TSlicedType> where TGeometry : Geometry
{
    IEnumerable<TSlicedType> Slice(TGeometry polygon);
}