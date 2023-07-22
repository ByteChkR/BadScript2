namespace BadScript2.Parser;

public class BadParameterMetaData
{
    public readonly string Description;
    public readonly string Type;

    public BadParameterMetaData(string type, string description)
    {
        Type = type;
        Description = description;
    }
}