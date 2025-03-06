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
using Services.Validators;
using Xunit.Abstractions;

namespace GeometryControllerTests;

public class RepeatPointsGeometryTest
{
    private Polygon _testRepeatPointsPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleRepeatPoints.geojson");
    private Polygon _testOriginalPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sample.geojson");
    
    private const double EpsilonCoordinateComparator = 1e-8;
    private const double Epsilon = 1e-15;
    
    private readonly ITestOutputHelper _output;

    public RepeatPointsGeometryTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test()
    {
        //Arrange
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
        var geometryController = new GeometryController<Polygon, Polygon, Polygon>(mockCreator.Object, mockRepository.Object, correctionService!);
        //Act
        var resultPolygon = geometryController.SaveGeometry(_testRepeatPointsPolygon, out string message);
        _output.WriteLine(message);
        Assert.Equal(_testOriginalPolygon, resultPolygon);
    }
    
    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}