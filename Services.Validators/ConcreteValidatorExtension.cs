using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.Validators.Interfaces;

namespace Services.Validators
{
    public static class ConcreteValidatorExtension
    {
        public static void AddConcreteValidator(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICoordinateComparator, EpsilonCoordinateComparator>();
            // serviceCollection.AddTransient<ICoordinateComparator, RefCoordinateComparator>();

            serviceCollection.AddTransient<IConcreteValidator<Polygon>, RepeatingPointsGeometryValidator>();
        }
    }
}