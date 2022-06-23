namespace BadScript2.Common.Logging;

public struct BadLog
{
    public readonly string Message;
    public readonly BadLogMask Mask;
    public readonly BadLogType Type;
    public readonly BadSourcePosition? Position;

    public BadLog(
        string message,
        BadLogMask? mask = null,
        BadSourcePosition? position = null,
        BadLogType type = BadLogType.Log)
    {
        Message = message;
        Type = type;
        Position = position;
        Mask = mask ?? BadLogMask.Default;
    }

    public static implicit operator BadLog(string message)
    {
        return new BadLog(message);
    }

    public override string ToString()
    {
        if (Position != null)
        {
            return $"[{Type}][{Mask}] {Message} at {Position.GetPositionInfo()}";
        }

        return $"[{Type}][{Mask}] {Message}";
    }
}