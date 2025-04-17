using NetTopologySuite.Geometries;

namespace UseCases.Interfaces;

public interface IGeometrySaver<TGeometryIn, TSliceType, TKey> where TGeometryIn : Geometry
{
    TKey SaveGeometry(TGeometryIn geometry, string layerAlias, int srid, out string validateResult);

    void StartTransaction();

    void CommitTransaction();

    void RollbackTransaction();
}