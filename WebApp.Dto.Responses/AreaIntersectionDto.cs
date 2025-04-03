using NetTopologySuite.Geometries;

namespace WebApp.Dto.Responses;

public class AreaIntersectionDto<TGeometry> where TGeometry : Geometry
{
    public string LayerAlias { get; set; }
    public string Properties { get; set; }
    public TGeometry Result { get; set; }

    public AreaIntersectionDto(string layerAlias, string properties, TGeometry result)
    {
        LayerAlias = layerAlias;
        Properties = properties;
        Result = result;
    }
}