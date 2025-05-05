namespace GeometrySaver.Tests;
using UseCases;
using UseCases.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetTopologySuite.Geometries;
using Services.GeometryCreators;
using Services.GeometryCreators.Interfaces;
using Services.GeometryFixers;
using Services.GeometrySlicers;
using Services.GeometryValidators;


public static class GeometrySaverBuilder
{
    private const double EpsilonCoordinateComparator = 1e-9;
    private const double Epsilon = 1e-15;

    
    public static IGeometrySaver<
        Geometry,
        FragmentWithNonRenderingBorder<Geometry, Geometry>,
        GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>> Build()
    {
        
        var mockRepository = new Mock<
            IRepository<
                GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>,
                GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
            >>();
        mockRepository
            .Setup(a => 
                a.Save(It.IsAny<
                    GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
                >(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(
                (GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>> input,
                    string _, string _) => input);
        
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddGeometrySlicers(Epsilon, GeometrySlicerType.OppositeSlicer, 1000);
        serviceCollection.AddGeometryFixer(EpsilonCoordinateComparator);
        serviceCollection.AddGeometryValidator(EpsilonCoordinateComparator);
        serviceCollection.AddGeometryWithFragmentsCreator();
        serviceCollection.AddGeometryCorrector();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var correctionService = serviceProvider.GetService<IGeometryCorrector<Geometry>>();
        return new GeometrySaver<
            Geometry,
            FragmentWithNonRenderingBorder<Geometry, Geometry>, 
            GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>
        >(
            serviceProvider.GetService<
                IGeometryWithFragmentsCreator<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>>()!, 
            mockRepository.Object,
            correctionService!);
    }
}