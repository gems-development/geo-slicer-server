using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryValidators.Interfaces;

namespace Services.GeometryValidators
{
    public static class GeometryValidatorExtension
    {
        public static void AddGeometryValidator(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISpecificValidator<Polygon>, NetTopologySuiteValidatorAdapter<Polygon>>();
            
            serviceCollection.AddTransient<ISpecificValidator<Polygon>, RepeatingPointsGeometryValidator>();
            
            serviceCollection.AddTransient<IGeometryValidator<Polygon>, GeometryValidator<Polygon>>();
        }
    }
}