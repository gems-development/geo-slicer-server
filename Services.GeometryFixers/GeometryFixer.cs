using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers;

public class GeometryFixer<TGeometry> : IGeometryFixer<TGeometry> where TGeometry : Geometry
{
    private IGeometryFixerFactory<TGeometry> _geometryFixerFactory;

    public GeometryFixer(IGeometryFixerFactory<TGeometry> geometryFixerFactory)
    {
        _geometryFixerFactory = geometryFixerFactory;
    }

    protected override TGeometry Fix(TGeometry geometry, GeometryValidateError[] geometryValidateErrors)
    {
        foreach (var error in geometryValidateErrors)
        {
            var fixer = _geometryFixerFactory.GetFixer(error);
            if (fixer == null)
                throw new ApplicationException($"There is no fixer for the error {error}");
            geometry = fixer.Fix(geometry);
        }
        return geometry;
    }
}