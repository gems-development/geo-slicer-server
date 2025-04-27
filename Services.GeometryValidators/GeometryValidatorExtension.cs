using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators;

public static class GeometryValidatorExtension
{
    public static void AddGeometryValidator(
        this IServiceCollection serviceCollection, double epsilon)
    {
        serviceCollection.AddTransient<ISpecificValidator<Geometry>, NtsSpecificValidatorAdapter>();
            
        serviceCollection.AddTransient<ISpecificValidator<Geometry>>(_ => new RepeatingPointsSpecificValidator(epsilon));
            
        serviceCollection.AddTransient<IGeometryValidator<Geometry>, GeometryValidator<Geometry>>();
    }
}