using UseCases;
using UseCases.Interfaces;
using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
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
    
    private const double EpsilonCoordinateComparator = 1e-8;
    private const double Epsilon = 1e-15;
    
    private readonly ITestOutputHelper _output;

    public RepeatPointsGeometryTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public IGeometrySaver<Polygon, Polygon, Polygon> GetGeometrySaver()
    {
        var mockCreator = new Mock<IGeometryWithFragmentsCreator<Polygon, Polygon>>();
        mockCreator.Setup(a => a.ToGeometryWithFragments(It.IsAny<Polygon>()))
            .Returns<Polygon>(input => new GeometryWithFragments<Polygon, Polygon>(input, Array. Empty<Polygon>())); 
        
        var mockRepository = new Mock<IRepository<GeometryWithFragments<Polygon, Polygon>, Polygon>>();
        mockRepository.Setup(a => a.Save(It.IsAny<GeometryWithFragments<Polygon, Polygon>>()))
            .Returns<GeometryWithFragments<Polygon, Polygon>>(input => input.Data);
        
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddGeometrySlicers(Epsilon);
        serviceCollection.AddGeometryFixer(EpsilonCoordinateComparator);
        serviceCollection.AddGeometryValidator(EpsilonCoordinateComparator);
        serviceCollection.AddGeometryCorrector();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var correctionService = serviceProvider.GetService<IGeometryCorrector<Polygon>>();
        return new GeometrySaver<Polygon, Polygon, Polygon>(mockCreator.Object, mockRepository.Object, correctionService!);
    }

    [Fact]
    public void TestRepeatingPoints()
    {
        //Arrange
        var geometryController = GetGeometrySaver();
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
        var geometryController = GetGeometrySaver();
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