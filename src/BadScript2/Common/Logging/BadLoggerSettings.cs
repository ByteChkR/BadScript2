using BadScript2.Settings;
using BadScript2.Utility;
namespace BadScript2.Common.Logging;

/// <summary>
///     Logger settings
/// </summary>
public class BadLoggerSettings : BadSettingsProvider<BadLoggerSettings>
{
    private readonly BadEditableSetting<BadLoggerSettings, string> m_ErrorBackgroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(ErrorBackgroundColor), "Black");


    private readonly BadEditableSetting<BadLoggerSettings, string> m_ErrorForegroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(ErrorForegroundColor), "White");

    private readonly BadEditableSetting<BadLoggerSettings, string> m_LogBackgroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(LogBackgroundColor), "Black");


    private readonly BadEditableSetting<BadLoggerSettings, string> m_LogForegroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(LogForegroundColor), "White");


    private readonly BadEditableSetting<BadLoggerSettings, string> m_WarnBackgroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(WarnBackgroundColor), "Black");


    private readonly BadEditableSetting<BadLoggerSettings, string> m_WarnForegroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(WarnForegroundColor), "White");

    public BadLoggerSettings() : base("Logging") { }

    public ConsoleColor LogForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_LogForegroundColor.GetValue()!, true);
        set => m_LogForegroundColor.Set(value.ToString());
    }

    public ConsoleColor LogBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_LogBackgroundColor.GetValue()!, true);
        set => m_LogBackgroundColor.Set(value.ToString());
    }

    public ConsoleColor WarnForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_WarnForegroundColor.GetValue()!, true);
        set => m_WarnForegroundColor.Set(value.ToString());
    }

    public ConsoleColor WarnBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_WarnBackgroundColor.GetValue()!, true);
        set => m_WarnBackgroundColor.Set(value.ToString());
    }

    public ConsoleColor ErrorForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_ErrorForegroundColor.GetValue()!, true);
        set => m_ErrorForegroundColor.Set(value.ToString());
    }

    public ConsoleColor ErrorBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_ErrorBackgroundColor.GetValue()!, true);
        set => m_ErrorBackgroundColor.Set(value.ToString());
    }
}