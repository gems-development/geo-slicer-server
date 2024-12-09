using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Slicers.Interfaces;

namespace Slicers
{
    public static class SlicerExtension
    {
        public static void AddSlicers(
            this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<ISlicer<Polygon, Polygon>,
                    MockSlicer<Polygon>>();
            
            serviceCollection
                .AddTransient<
                    ISlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>,
                    GeometryWithFragmentsSlicer>();
        }
    }
}