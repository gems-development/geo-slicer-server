using Microsoft.Extensions.DependencyInjection;
using Slicers.Interfaces;

namespace Slicers
{
    public static class MockSlicerExtension
    {
        public static void AddToServiceCollection<TGeometry>(
            this MockSlicer<TGeometry> mockSlicer, 
            IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<ISlicer<TGeometry, TGeometry>,
                    MockSlicer<TGeometry>>();
        }
    }
}