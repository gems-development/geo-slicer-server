using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.GeometryCreators.Interfaces;

namespace Services.GeometryCreators
{
    public static class GeometryWithFragmentsCreatorExtension
    {
        public static void AddGeometryWithFragmentsCreator(
            this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<
                    IGeometryWithFragmentsCreator<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>,
                    GeometryWithFragmentsCreator<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>>();
        }
    }
}