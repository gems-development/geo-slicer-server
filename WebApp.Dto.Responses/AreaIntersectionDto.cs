using NetTopologySuite.Geometries;

namespace WebApp.Dto.Responses;

public class AreaIntersectionDto<TGeometry> where TGeometry : Geometry
{
    public string LayerAlias { get; set; }
    public string LayerName { get; set; }
    public TGeometry Result { get; set; }

    public AreaIntersectionDto(string layerAlias, string layerName, TGeometry result)
    {
        LayerAlias = layerAlias;
        LayerName = layerName;
        Result = result;
    }
}