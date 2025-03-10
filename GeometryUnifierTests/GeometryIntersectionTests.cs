using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using DataAccess.Repositories.GeometryUnion;
using DomainModels;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace GeometryUnifierTests;

public class GeometryIntersectionTests
{
    [Fact]
    public void Test()
    {
        Polygon baikalTriangle = ReadGeometryFromFile<Polygon>("TestFiles/baikalTriangle.geojson");
        String connectionString = "Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin";
        var pgContext = new PostgreApplicationContext(connectionString);
        var polygons = pgContext.GeometryOriginals.Where(o => o.Id == 13)
            .Select(o => o.Data.Intersection(baikalTriangle)).ToList();
        int i = 0;
        foreach (var polygon in polygons)
        {
            var polygonJson = new GeoJsonWriter().Write(polygon);
            File.WriteAllText("baikalIntersects" + i, polygonJson);
            i++;
        }
    }

    private static T ReadGeometryFromFile<T>(string path) where T : class
    {
        string geoJson = File.ReadAllText(path);
        var geometry = new GeoJsonReader().Read<T>(geoJson);
        return geometry;
    }
}