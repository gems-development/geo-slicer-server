using ConsoleApp.Controllers;
using ConsoleApp.Services;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using GeoSlicer.Config;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Services.Creators.Interfaces;
using Services.Fixers;
using Services.Fixers.Interfaces;
using Services.Validators;
using Services.Validators.Interfaces;
using Xunit.Abstractions;

namespace GeometryControllerTests;

public class RepeatPointsGeometryTest
{
    private Polygon _testRepeatPointsPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleRepeatPoints.geojson");
    private Polygon _testOriginalPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sample.geojson");
    private Polygon _testHoleNotInPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleHoleOutside.geojson");
    
    private const double EpsilonCoordinateComparator = 1e-8;
    private const double Epsilon = 1e-15;
    
    private readonly ITestOutputHelper _output;

    public RepeatPointsGeometryTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public GeometryController<Polygon, Polygon, Polygon> GetGeometryController()
    {
        var mockCreator = new Mock<IGeometryWithFragmentsCreator<Polygon, Polygon>>();
        mockCreator.Setup(a => a.ToGeometryWithFragments(It.IsAny<Polygon>()))
            .Returns<Polygon>(input => new GeometryWithFragments<Polygon, Polygon>(input, Array. Empty<Polygon>())); 
        
        var mockRepository = new Mock<IRepository<GeometryWithFragments<Polygon, Polygon>, Polygon>>();
        mockRepository.Setup(a => a.Save(It.IsAny<GeometryWithFragments<Polygon, Polygon>>()))
            .Returns<GeometryWithFragments<Polygon, Polygon>>(input => input.Data);
        
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddAlgorithms(EpsilonCoordinateComparator, Epsilon);
        serviceCollection.AddGeometryFixer();
        serviceCollection.AddGeometryValidator();
        serviceCollection.AddCorrectionService();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var correctionService = serviceProvider.GetService<CorrectionService<Polygon>>();
        return new GeometryController<Polygon, Polygon, Polygon>(mockCreator.Object, mockRepository.Object, correctionService!);
    }

    [Fact]
    public void TestRepeatingPoints()
    {
        //Arrange
        var geometryController = GetGeometryController();
        //Act
        var resultPolygon = geometryController.SaveGeometry(_testRepeatPointsPolygon, out string message);
        //Assert
        Assert.Equal(_testOriginalPolygon, resultPolygon);
        Assert.Equal("Validate errors: GeometryHasRepeatingPoints", message);
    }
    
    [Fact]
    public void TestHoleNotInPolygon()
    {
        //Arrange
        var geometryController = GetGeometryController();
        string message = string.Empty;
        //Act + assert
        Exception exception = Assert.Throws<Exception>(() => geometryController.SaveGeometry(_testHoleNotInPolygon, out message));
        Assert.Equal("Validate errors: HoleOutsideShell\nThere is no fixer for the error HoleOutsideShell", exception.Message);
        Assert.Equal("Validate errors: HoleOutsideShell", message);
    }
    
    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}