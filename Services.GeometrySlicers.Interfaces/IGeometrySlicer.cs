using NetTopologySuite.Geometries;

namespace Services.GeometrySlicers.Interfaces;

//реализация может не учитывать srid входной фигуры, так как в GeometryWithFragmentsCreator он учитывается
public interface IGeometrySlicer<TGeometry, TSlicedType> where TGeometry : Geometry
{
    IEnumerable<TSlicedType> Slice(TGeometry polygon);
}