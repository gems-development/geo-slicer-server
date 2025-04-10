using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public static class GeometryValidatorExtension
{
    public static void AddGeometryValidator(
        this IServiceCollection serviceCollection, double epsilon)
    {
        serviceCollection.AddTransient<ISpecificValidator<Polygon>, NetTopologySuiteValidatorAdapter<Polygon>>();
            
        serviceCollection.AddTransient<ISpecificValidator<Polygon>>(_ => new RepeatingPointsGeometryValidator(epsilon));
            
        serviceCollection.AddTransient<IGeometryValidator<Polygon>, GeometryValidator<Polygon>>();
    }
}