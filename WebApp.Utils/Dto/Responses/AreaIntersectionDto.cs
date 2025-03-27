using NetTopologySuite.Geometries;

namespace WebApp.Utils.Dto.Responses;

public class AreaIntersectionDto<TGeometry> where TGeometry : Geometry
{
    public string LayerName { get; set; }
    public TGeometry Result { get; set; }

    public AreaIntersectionDto(string layerName, TGeometry result)
    {
        LayerName = layerName;
        Result = result;
    }
}