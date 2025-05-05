namespace Services.GeometryValidateErrors;

public record GeometryValidateError
{
    public GeometryValidateErrorType Type { get; init; }
    public string Message { get; init; }

    public GeometryValidateError(GeometryValidateErrorType type, string message)
    {
        Type = type;
        Message = message;
    }
}