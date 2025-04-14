namespace WebApp.Dto.Responses;

public class ClickInfoDto<TInfo>
{
    public string LayerAlias { get; set; }
    public string Properties { get; set; }
    public TInfo Id { get; set; }

    public ClickInfoDto(string layerAlias, string properties, TInfo id)
    {
        LayerAlias = layerAlias;
        Properties = properties;
        Id = id;
    }
}