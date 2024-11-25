using System;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using Services.Interfaces;

namespace Services
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