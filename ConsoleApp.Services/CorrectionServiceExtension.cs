using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace ConsoleApp.Services
{
    public static class CorrectionServiceExtension
    {
        public static void AddCorrectionService(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<CorrectionService<Polygon>, CorrectionService<Polygon>>();
        }
    }
}