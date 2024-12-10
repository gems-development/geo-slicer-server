using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace ConsoleApp.Controllers
{
    public static class GeometryControllerExtension
    {
        public static void AddGeometryController(
            this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<GeometryController<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>, int>,
                    GeometryController<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>, int>>();
        }
    }
}