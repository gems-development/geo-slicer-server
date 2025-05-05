using UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace UseCases;

public static class GeometryCorrectorExtension
{
    public static void AddGeometryCorrector(
        this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGeometryCorrector<Geometry>, GeometryCorrector<Geometry>>();
    }
}