namespace BadScript2.Common;

public abstract class BadParseException : Exception
{
    protected BadParseException(string message, BadSourcePosition position) : base(GetMessage(message, position))
    {
        Position = position;
    }

    protected BadParseException(string message, BadSourcePosition position, Exception inner) : base(
        GetMessage(message, position),
        inner
    )
    {
        Position = position;
    }

    public BadSourcePosition Position { get; }

    private static string GetMessage(string message, BadSourcePosition position)
    {
        return $"{message} at {position.GetExcerpt(10, 10)} in {position.GetPositionInfo()}";
    }
}