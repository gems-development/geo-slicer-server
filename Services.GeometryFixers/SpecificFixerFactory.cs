using NetTopologySuite.Geometries;
using Services.GeometryFixers.Interfaces;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers;

public class SpecificFixerFactory : ISpecificFixerFactory<Geometry>
{
    private readonly IDictionary<GeometryValidateErrorType, ISpecificFixer<Geometry>> _specificFixers =
        new Dictionary<GeometryValidateErrorType, ISpecificFixer<Geometry>>();

    public SpecificFixerFactory(double epsilon)
    {
        _specificFixers[GeometryValidateErrorType.GeometryHasRepeatingPoints] = new RepeatingPointsFixer(epsilon);
    }

    public ISpecificFixer<Geometry>? GetFixer(GeometryValidateErrorType geometryValidateErrorType)
    {
        return _specificFixers.TryGetValue(geometryValidateErrorType, out var fixer) ? fixer : null;
    }
}