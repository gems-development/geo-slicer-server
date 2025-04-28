using System.Collections.ObjectModel;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json.Linq;

namespace Utils;

public class GeometryReader
{

    public Collection<IFeature> ReadGeometriesFromFile(string path)
    {
        string geoJson = File.ReadAllText(path);
        
        var geoJsonReader = new GeoJsonReader();
        
        var jsonObject = JObject.Parse(geoJson);
        
        string geoJsonType = jsonObject["type"].ToString();

        return geoJsonType switch
        {
            "FeatureCollection" => geoJsonReader.Read<FeatureCollection>(geoJson),
            "Feature" => new Collection<IFeature> { geoJsonReader.Read<Feature>(geoJson) },
            "Point" or "LineString" or "Polygon" or "MultiPolygon" 
                or "MultiLineString" or "MultiPoint" or "GeometryCollection"
                => new Collection<IFeature>
                {
                    new Feature(geoJsonReader.Read<Geometry>(geoJson), new AttributesTable())
                },
            _ => throw new NotSupportedException($"Тип GeoJSON '{geoJsonType}' не поддерживается.")
        };
    }
}