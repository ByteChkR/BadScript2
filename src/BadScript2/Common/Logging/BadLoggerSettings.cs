using BadScript2.Settings;
using BadScript2.Utility;

namespace BadScript2.Common.Logging;

/// <summary>
///     Logger settings
/// </summary>
public class BadLoggerSettings : BadSettingsProvider<BadLoggerSettings>
{
    private BadSettings? m_ErrorBackgroundColorObj;


    private BadSettings? m_ErrorForegroundColorObj;


    private BadSettings? m_LogBackgroundColorObj;

    private BadSettings? m_LogForegroundColorObj;


    private BadSettings? m_WarnBackgroundColorObj;


    private BadSettings? m_WarnForegroundColorObj;
    public BadLoggerSettings() : base("Logging") { }

    private BadSettings? LogForegroundColorObj =>
        m_LogForegroundColorObj ??= Settings?.GetProperty(nameof(LogForegroundColor));

    public ConsoleColor LogForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(
            LogForegroundColorObj?.GetValue<string>() ?? "White",
            true
        );
        set => LogForegroundColorObj?.SetValue(value.ToString());
    }

    private BadSettings? LogBackgroundColorObj =>
        m_LogBackgroundColorObj ??= Settings?.GetProperty(nameof(LogBackgroundColor));

    public ConsoleColor LogBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(
            LogBackgroundColorObj?.GetValue<string>() ?? "Black",
            true
        );
        set => LogBackgroundColorObj?.SetValue(value.ToString());
    }

    private BadSettings? WarnForegroundColorObj =>
        m_WarnForegroundColorObj ??= Settings?.GetProperty(nameof(WarnForegroundColor));

    public ConsoleColor WarnForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(
            WarnForegroundColorObj?.GetValue<string>() ?? "White",
            true
        );
        set => WarnForegroundColorObj?.SetValue(value.ToString());
    }

    private BadSettings? WarnBackgroundColorObj =>
        m_WarnBackgroundColorObj ??= Settings?.GetProperty(nameof(WarnBackgroundColor));

    public ConsoleColor WarnBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(
            WarnBackgroundColorObj?.GetValue<string>() ?? "Black",
            true
        );
        set => WarnBackgroundColorObj?.SetValue(value.ToString());
    }

    private BadSettings? ErrorForegroundColorObj =>
        m_ErrorForegroundColorObj ??= Settings?.GetProperty(nameof(ErrorForegroundColor));

    public ConsoleColor ErrorForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(
            ErrorForegroundColorObj?.GetValue<string>() ?? "White",
            true
        );
        set => ErrorForegroundColorObj?.SetValue(value.ToString());
    }

    private BadSettings? ErrorBackgroundColorObj =>
        m_ErrorBackgroundColorObj ??= Settings?.GetProperty(nameof(ErrorBackgroundColor));

    public ConsoleColor ErrorBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(
            ErrorBackgroundColorObj?.GetValue<string>() ?? "Black",
            true
        );
        set => ErrorBackgroundColorObj?.SetValue(value.ToString());
    }
}