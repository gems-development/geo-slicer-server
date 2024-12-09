using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.Validators.Interfaces;

namespace Services.Validators
{
    public static class ValidatorExtension
    {
        public static void AddGeometryValidator(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IConcreteValidator<Polygon>, NetTopologySuiteValidatorAdapter<Polygon>>();
            serviceCollection.AddTransient<IConcreteValidator<Polygon>, RepeatingPointsGeometryValidator>();

            serviceCollection.AddTransient<IGeometryValidator<Polygon>, GeometryValidator<Polygon>>();
        }
    }
}