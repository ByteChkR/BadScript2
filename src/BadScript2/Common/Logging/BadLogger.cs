namespace BadScript2.Common.Logging;

public static class BadLogger
{
    public static event Action<BadLog>? OnLog;

    private static void Write(BadLog message)
    {
        OnLog?.Invoke(message);
    }

    public static void Log(string message)
    {
        Write(new BadLog(message));
    }

    public static void Log(string message, BadLogMask mask)
    {
        Write(new BadLog(message, mask));
    }

    public static void Log(string message, BadLogMask mask, BadSourcePosition position)
    {
        Write(new BadLog(message, mask, position));
    }

    public static void Warn(string message)
    {
        Write(new BadLog(message, null, null, BadLogType.Warning));
    }

    public static void Warn(string message, BadLogMask mask)
    {
        Write(new BadLog(message, mask, null, BadLogType.Warning));
    }

    public static void Warn(string message, BadLogMask mask, BadSourcePosition position)
    {
        Write(new BadLog(message, mask, position, BadLogType.Warning));
    }

    public static void Error(string message)
    {
        Write(new BadLog(message, null, null, BadLogType.Warning));
    }

    public static void Error(string message, BadLogMask mask)
    {
        Write(new BadLog(message, mask, null, BadLogType.Error));
    }

    public static void Error(string message, BadLogMask mask, BadSourcePosition position)
    {
        Write(new BadLog(message, mask, position, BadLogType.Error));
    }
}