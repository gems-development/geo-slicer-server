using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Xunit.Abstractions;

namespace GeometrySaver.Tests;

public class RepeatPointsGeometryTest
{
    
    private Polygon _testRepeatPointsPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleRepeatPoints.geojson");
    private Polygon _testOriginalPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sample.geojson");
    private Polygon _testHoleNotInPolygon = ReadGeometryFromFile<Polygon>("TestFiles\\sampleHoleOutside.geojson");
    
    private readonly ITestOutputHelper _output;

    public RepeatPointsGeometryTest(ITestOutputHelper output)
    {
        _output = output;
    }
    

    [Fact]
    public void TestRepeatingPoints()
    {
        //Arrange
        var geometryController = GeometrySaverBuilder.Build();
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
        var geometryController = GeometrySaverBuilder.Build();
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