using BadScript2.Settings;

namespace BadScript2.Common.Logging;

public class BadLoggerSettings : BadSettingsProvider<BadLoggerSettings>
{
    public BadLoggerSettings() : base("Logging") { }

    private BadSettings? m_LogForegroundColorObj;

    private BadSettings? LogForegroundColorObj =>
        m_LogForegroundColorObj ??= Settings?.GetProperty(nameof(LogForegroundColor));
    public ConsoleColor LogForegroundColor
    {
        get => Enum.Parse<ConsoleColor>(
            LogForegroundColorObj?.GetValue<string>() ?? "White",
            true
        );
        set => LogForegroundColorObj?.SetValue(value.ToString());
    }

    
    private BadSettings? m_LogBackgroundColorObj;
    
    private BadSettings? LogBackgroundColorObj => m_LogBackgroundColorObj ??= Settings?.GetProperty(nameof(LogBackgroundColor));
    public ConsoleColor LogBackgroundColor
    {
        get => Enum.Parse<ConsoleColor>(
            LogBackgroundColorObj?.GetValue<string>() ?? "Black",
            true
        );
        set => LogBackgroundColorObj?.SetValue(value.ToString());
    }

    
    private BadSettings? m_WarnForegroundColorObj;
    
    private BadSettings? WarnForegroundColorObj => m_WarnForegroundColorObj ??= Settings?.GetProperty(nameof(WarnForegroundColor));
    public ConsoleColor WarnForegroundColor
    {
        get => Enum.Parse<ConsoleColor>(
            WarnForegroundColorObj?.GetValue<string>() ?? "White",
            true
        );
        set => WarnForegroundColorObj?.SetValue(value.ToString());
    }

    
    private BadSettings? m_WarnBackgroundColorObj;
    
    private BadSettings? WarnBackgroundColorObj => m_WarnBackgroundColorObj ??= Settings?.GetProperty(nameof(WarnBackgroundColor));
    public ConsoleColor WarnBackgroundColor
    {
        get => Enum.Parse<ConsoleColor>(
            WarnBackgroundColorObj?.GetValue<string>() ?? "Black",
            true
        );
        set => WarnBackgroundColorObj?.SetValue(value.ToString());
    }

    
    private BadSettings? m_ErrorForegroundColorObj;
    
    private BadSettings? ErrorForegroundColorObj => m_ErrorForegroundColorObj ??= Settings?.GetProperty(nameof(ErrorForegroundColor));
    public ConsoleColor ErrorForegroundColor
    {
        get => Enum.Parse<ConsoleColor>(
            ErrorForegroundColorObj?.GetValue<string>() ?? "White",
            true
        );
        set => ErrorForegroundColorObj?.SetValue(value.ToString());
    }

    
    private BadSettings? m_ErrorBackgroundColorObj;
    
    private BadSettings? ErrorBackgroundColorObj => m_ErrorBackgroundColorObj ??= Settings?.GetProperty(nameof(ErrorBackgroundColor));
    public ConsoleColor ErrorBackgroundColor
    {
        get => Enum.Parse<ConsoleColor>(
            ErrorBackgroundColorObj?.GetValue<string>() ?? "Black",
            true
        );
        set => ErrorBackgroundColorObj?.SetValue(value.ToString());
    }
}