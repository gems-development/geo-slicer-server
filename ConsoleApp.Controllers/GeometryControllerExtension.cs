using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp.Controllers
{
    public static class GeometryControllerExtension
    {
        public static void AddToServiceCollection<TGeometryIn, TSliceType, TKey>(
            this GeometryController<TGeometryIn, TSliceType, TKey> geometryController,
            IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<GeometryController<TGeometryIn, TSliceType, TKey>>();
        }
    }
}