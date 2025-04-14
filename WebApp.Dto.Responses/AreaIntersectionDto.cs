using NetTopologySuite.Geometries;

namespace WebApp.Dto.Responses;

public class AreaIntersectionDto<TGeometry> where TGeometry : Geometry
{
    public int Id { get; set; }
    public string LayerAlias { get; set; }
    public string Properties { get; set; }
    public TGeometry Result { get; set; }

    public AreaIntersectionDto(int id, string layerAlias, string properties, TGeometry result)
    {
        Id = id;
        LayerAlias = layerAlias;
        Properties = properties;
        Result = result;
    }
}