using BadScript2.Settings;
using BadScript2.Utility;

namespace BadScript2.Common.Logging;

/// <summary>
///     Logger settings
/// </summary>
public class BadLoggerSettings : BadSettingsProvider<BadLoggerSettings>
{
    /// <summary>
    /// Backing Field of <see cref="LogForegroundColor"/>
    /// </summary>
    private readonly BadEditableSetting<BadLoggerSettings, string> m_ErrorBackgroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(ErrorBackgroundColor), "Black");

    /// <summary>
    /// Backing Field of <see cref="LogBackgroundColor"/>
    /// </summary>
    private readonly BadEditableSetting<BadLoggerSettings, string> m_ErrorForegroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(ErrorForegroundColor), "White");

    /// <summary>
    /// Backing Field of <see cref="LogForegroundColor"/>
    /// </summary>
    private readonly BadEditableSetting<BadLoggerSettings, string> m_LogBackgroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(LogBackgroundColor), "Black");


    /// <summary>
    /// Backing Field of <see cref="LogForegroundColor"/>
    /// </summary>
    private readonly BadEditableSetting<BadLoggerSettings, string> m_LogForegroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(LogForegroundColor), "White");


    /// <summary>
    /// Backing Field of <see cref="WarnBackgroundColor"/>
    /// </summary>
    private readonly BadEditableSetting<BadLoggerSettings, string> m_WarnBackgroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(WarnBackgroundColor), "Black");


    /// <summary>
    /// Backing Field of <see cref="WarnForegroundColor"/>
    /// </summary>
    private readonly BadEditableSetting<BadLoggerSettings, string> m_WarnForegroundColor =
        new BadEditableSetting<BadLoggerSettings, string>(nameof(WarnForegroundColor), "White");

    /// <summary>
    /// Creates a new instance of the Logger Settings
    /// </summary>
    public BadLoggerSettings() : base("Logging") { }

    /// <summary>
    /// The foreground color of the log messages
    /// </summary>
    public ConsoleColor LogForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_LogForegroundColor.GetValue()!, true);
        set => m_LogForegroundColor.Set(value.ToString());
    }

    /// <summary>
    /// The background color of the log messages
    /// </summary>
    public ConsoleColor LogBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_LogBackgroundColor.GetValue()!, true);
        set => m_LogBackgroundColor.Set(value.ToString());
    }

    /// <summary>
    /// The foreground color of the warning messages
    /// </summary>
    public ConsoleColor WarnForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_WarnForegroundColor.GetValue()!, true);
        set => m_WarnForegroundColor.Set(value.ToString());
    }

    /// <summary>
    /// The background color of the warning messages
    /// </summary>
    public ConsoleColor WarnBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_WarnBackgroundColor.GetValue()!, true);
        set => m_WarnBackgroundColor.Set(value.ToString());
    }

    /// <summary>
    /// The foreground color of the error messages
    /// </summary>
    public ConsoleColor ErrorForegroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_ErrorForegroundColor.GetValue()!, true);
        set => m_ErrorForegroundColor.Set(value.ToString());
    }

    /// <summary>
    /// The background color of the error messages
    /// </summary>
    public ConsoleColor ErrorBackgroundColor
    {
        get => BadEnum.Parse<ConsoleColor>(m_ErrorBackgroundColor.GetValue()!, true);
        set => m_ErrorBackgroundColor.Set(value.ToString());
    }
}