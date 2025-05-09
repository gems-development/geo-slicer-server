using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Xunit.Abstractions;

namespace GeometrySaver.Tests;

public class InvalidGeometryCollectionTest
{
    private GeometryCollection _validGeometryCollection = (GeometryCollection)
        ReadGeometryFromFile<FeatureCollection>("TestFiles\\testGeometryCollection.geojson")[0].Geometry;
    
    private GeometryCollection _invalidGeometryCollection = (GeometryCollection)
        ReadGeometryFromFile<FeatureCollection>("TestFiles\\testInvalidRepeatingPointsGeometryCollection.geojson")[0].Geometry;
    
    private readonly ITestOutputHelper _output;

    public InvalidGeometryCollectionTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void TestRepeatingPoints()
    {
        //Arrange
        var geometryController = GeometrySaverBuilder.Build();
        _invalidGeometryCollection.SRID = 0;
        _validGeometryCollection.SRID = 0;
        //Act
        var resultCollection = geometryController
            .SaveGeometry(_invalidGeometryCollection, "", "", out string message);
        //Assert
        var resultGeojson = new GeoJsonWriter().Write((GeometryCollection) resultCollection.Data);
        var validGeometryGeojson = new GeoJsonWriter().Write(_validGeometryCollection);
        Assert.Equal(validGeometryGeojson, resultGeojson);
        Assert.Equal(
            "\nValidate errors: GeometryHasRepeatingPoints: Equals points at 8 and 9\n: (109.2226941, 56.63581), (109.2226941, 56.63581)\nValidate errors: GeometryValid: no errors were found", 
            message);
        //_output.WriteLine(message);
    }
    
    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}