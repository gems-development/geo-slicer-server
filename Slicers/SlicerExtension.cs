using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Slicers.Interfaces;
using GeoSlicer.DivideAndRuleSlicers.OppositesSlicer;

namespace Slicers
{
    public static class SlicerExtension
    {
        public static void AddSlicers(
            this IServiceCollection serviceCollection, Slicer slicer)
        {
            serviceCollection
                .AddTransient<ISlicer<Polygon, Polygon>>(provider => new OppositesSlicerAdapter(slicer));
            
            serviceCollection
                .AddTransient<
                    ISlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>,
                    GeometryWithFragmentsSlicer>();
        }
    }
}