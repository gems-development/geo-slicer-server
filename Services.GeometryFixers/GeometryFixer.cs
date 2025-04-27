using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers;

public class GeometryFixer<TGeometry> : IGeometryFixer<TGeometry> where TGeometry : Geometry
{
    private ISpecificFixerFactory<TGeometry> _specificFixerFactory;

    public GeometryFixer(ISpecificFixerFactory<TGeometry> specificFixerFactory)
    {
        _specificFixerFactory = specificFixerFactory;
    }

    protected override TGeometry Fix(TGeometry geometry, GeometryValidateErrorType[] geometryValidateErrors)
    {
        foreach (var error in geometryValidateErrors)
        {
            var fixer = _specificFixerFactory.GetFixer(error);
            if (fixer == null)
                throw new ApplicationException($"There is no fixer for the error {error}");
            geometry = fixer.Fix(geometry);
        }
        return geometry;
    }
}