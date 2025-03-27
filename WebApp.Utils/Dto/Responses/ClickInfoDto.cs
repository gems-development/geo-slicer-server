namespace WebApp.Utils.Dto.Responses;

public class ClickInfoDto<TInfo>
{
    public string LayerName { get; set; }
    public TInfo Id { get; set; }

    public ClickInfoDto(string layerName, TInfo id)
    {
        LayerName = layerName;
        Id = id;
    }
}