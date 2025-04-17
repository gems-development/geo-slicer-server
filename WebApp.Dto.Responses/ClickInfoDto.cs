namespace WebApp.Dto.Responses;

public class ClickInfoDto<TId>
{
    public string LayerAlias { get; set; }
    public string Properties { get; set; }
    public TId Id { get; set; }

    public ClickInfoDto(string layerAlias, string properties, TId id)
    {
        LayerAlias = layerAlias;
        Properties = properties;
        Id = id;
    }
}