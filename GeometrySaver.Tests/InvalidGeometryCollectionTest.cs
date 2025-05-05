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
        //Assert.True(_validGeometryCollection.Equals((GeometryCollection) resultCollection.Data));
        var resCol = new GeoJsonWriter().Write((GeometryCollection) resultCollection.Data);
        var inv = new GeoJsonWriter().Write(_validGeometryCollection);
        //File.WriteAllText("TestFiles\\123.geojson", inv);
        //File.WriteAllText("TestFiles\\456.geojson", resCol);
        Assert.Equal(inv, resCol);
        _output.WriteLine(message);
    }
    
    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}