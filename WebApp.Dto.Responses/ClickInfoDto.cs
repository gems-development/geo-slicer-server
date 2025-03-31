namespace WebApp.Dto.Responses;

public class ClickInfoDto<TInfo>
{
    public string LayerAlias { get; set; }
    public string LayerName { get; set; }
    public TInfo Id { get; set; }

    public ClickInfoDto(string layerAlias, string layerName, TInfo id)
    {
        LayerAlias = layerAlias;
        LayerName = layerName;
        Id = id;
    }
}