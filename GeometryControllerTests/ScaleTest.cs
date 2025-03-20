using DataAccess.PostgreSql;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using WebAppUseCases.Repositories;

namespace GeometryControllerTests;

public class ScaleTest
{
    private Polygon _baikal = 
        (Polygon) ReadGeometryFromFile<MultiPolygon>("TestFiles\\baikal_multy_polygon.geojson").Geometries.First();
    
    [Fact]
    public async void ScaleTestMethod()
    {
        PostgreApplicationContext context =
            new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");
        GeometryRepository geometryRepo = new(context);
        
        Coordinate pointLeftBottom = new (101.4089, 50.0641);
        Coordinate pointRightTop = new (114.1606, 56.9720);
        double epsilon = 0.8 / 1500;
        //epsilon = 0;
        Coordinate coordinateLeftTop = new Coordinate(pointLeftBottom.X, pointRightTop.Y);
        Coordinate coordinateRightBottom = new Coordinate(pointRightTop.X, pointLeftBottom.Y);
        LinearRing ring = new LinearRing(new []
        {
            pointLeftBottom,
            coordinateLeftTop,
            pointRightTop,
            coordinateRightBottom, 
            pointLeftBottom
        });
        Polygon polygon = new Polygon(ring);
        
        var res = await geometryRepo.GetSimplifiedGeometryByPolygon(polygon, epsilon);
        String resJson = new GeoJsonWriter().Write(res);
        File.WriteAllText("baikalScale.geojson", resJson);
    }

    
    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }

    [Fact]
    public void ParseKazan()
    {
        var featureCollection = ReadGeometryFromFile<FeatureCollection>("TestFiles\\kazan.geojson");
        var polygons = featureCollection
            .Select(a => (MultiPolygon) a.Geometry)
            .First()
            .Geometries
            .Select(b => (Polygon) b)
            .ToList();
        var geoJsonWriter = new GeoJsonWriter();
        for (int i = 0; i < polygons.Count; i++)
        {
            String polygonJson = geoJsonWriter.Write(polygons[i]);
            File.WriteAllText($"parseKazan\\kazan{i}.geojson", polygonJson);
        }
    }
}