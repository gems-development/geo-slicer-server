using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite.Geometries;

namespace ConsoleApp.Controllers
{
    public static class GeometryControllerExtension
    {
        public static void AddGeometryController(
            this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<GeometryController<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>, int>,
                    GeometryController<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>, int>>();
        }
    }
}