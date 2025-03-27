using System.Text.Json.Serialization;

namespace WebApp.Utils.Dto.Responses;

public class ClickInfoDto<TInfo>
{
    public string LayerName { get; set; }
    public TInfo Id { get; set; }

    [JsonConstructor]
    public ClickInfoDto(string layerName, TInfo id)
    {
        LayerName = layerName;
        Id = id;
    }
}