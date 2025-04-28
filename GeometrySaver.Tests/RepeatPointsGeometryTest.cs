using UseCases;
using UseCases.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using GeometrySlicerTypes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Services.GeometryCreators;
using Services.GeometryCreators.Interfaces;
using Services.GeometryFixers;
using Services.GeometrySlicers;
using Services.GeometryValidators;
using Xunit.Abstractions;

namespace GeometrySaver.Tests;

public class RepeatPointsGeometryTest
{
    
    private Polygon _testRepeatPointsPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleRepeatPoints.geojson");
    private Polygon _testOriginalPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sample.geojson");
    private Polygon _testHoleNotInPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleHoleOutside.geojson");
    
    private const double EpsilonCoordinateComparator = 1e-9;
    private const double Epsilon = 1e-15;
    
    private readonly ITestOutputHelper _output;

    public RepeatPointsGeometryTest(ITestOutputHelper output)
    {
        _output = output;
    }

    
    public IGeometrySaver<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>, GeometryWithFragments<Geometry, FragmentWithNonRenderingBorder<Geometry, Geometry>>> GetGeometrySaver()
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

    [Fact]
    public void TestRepeatingPoints()
    {
        //Arrange
        var geometryController = GetGeometrySaver();
        //Act
        var resultPolygon = geometryController
            .SaveGeometry(_testRepeatPointsPolygon, "", "", out string message);
        //Assert
        Assert.Equal(_testOriginalPolygon, (Polygon) resultPolygon.Data);
        Assert.Equal("\nValidate errors: GeometryHasRepeatingPoints: Equals points at 3 and 4" +
                     "\n: (-4, -4), (-4, -4)\nValidate errors: GeometryValid: no errors were found",
            message);
        
        //_output.WriteLine(message);
    }
    
    [Fact]
    public void TestHoleNotInPolygon()
    {
        //Arrange
        var geometryController = GetGeometrySaver();
        string message = string.Empty;
        //Act + assert
        Exception exception = Assert.Throws<Exception>(() => geometryController.SaveGeometry(_testHoleNotInPolygon, "", "", out message));
        Assert.Equal("\nValidate errors: HoleOutsideShell: Hole lies outside shell\nThere is no fixer for the error HoleOutsideShell", exception.Message);
        Assert.Equal("\nValidate errors: HoleOutsideShell: Hole lies outside shell", message);
        
        //_output.WriteLine(message);
    }
    
    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}