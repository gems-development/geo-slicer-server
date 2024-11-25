using Microsoft.Extensions.DependencyInjection;
using Services.Creators.Interfaces;

namespace Services.Creators
{
    public static class GeometryWithFragmentsCreatorExtension
    {
        public static void AddToServiceCollection<TGeometry, TSlicedType>(
            this GeometryWithFragmentsCreator<TGeometry, TSlicedType> geometryWithFragmentsCreator, 
            IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IGeometryWithFragmentsCreator<TGeometry, TSlicedType>,
                    GeometryWithFragmentsCreator<TGeometry, TSlicedType>>();
        }
    }
}