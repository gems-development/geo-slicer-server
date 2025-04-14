using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers;

public class GeometryFixerFactory : IGeometryFixerFactory<Polygon>
{
    private readonly IDictionary<GeometryValidateError, ISpecificFixer<Polygon>> _concreteFixers =
        new Dictionary<GeometryValidateError, ISpecificFixer<Polygon>>();

    public GeometryFixerFactory(double epsilon)
    {
        _concreteFixers[GeometryValidateError.GeometryHasRepeatingPoints] = new RepeatingPointsFixer(epsilon);
    }

    public ISpecificFixer<Polygon>? GetFixer(GeometryValidateError geometryValidateError)
    {
        return _concreteFixers.TryGetValue(geometryValidateError, out var fixer) ? fixer : null;
    }
}