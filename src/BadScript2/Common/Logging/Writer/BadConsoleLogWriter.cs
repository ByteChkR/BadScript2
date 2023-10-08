using BadScript2.ConsoleAbstraction;

namespace BadScript2.Common.Logging.Writer;

/// <summary>
///     Console Log Writer
/// </summary>
public class BadConsoleLogWriter : BadLogWriter
{
    /// <summary>
    ///     Writes a log message to the log writer
    /// </summary>
    /// <param name="log">The log to be written</param>
    /// <exception cref="ArgumentOutOfRangeException">Gets raised if the BadLogType is unsupported</exception>
    protected override void Write(BadLog log)
    {
        ConsoleColor fg = BadConsole.ForegroundColor;
        ConsoleColor bg = BadConsole.BackgroundColor;

        switch (log.Type)
        {
            case BadLogType.Log:
                BadConsole.ForegroundColor = BadLoggerSettings.Instance.LogForegroundColor;
                BadConsole.BackgroundColor = BadLoggerSettings.Instance.LogBackgroundColor;

                break;
            case BadLogType.Warning:
                BadConsole.ForegroundColor = BadLoggerSettings.Instance.WarnForegroundColor;
                BadConsole.BackgroundColor = BadLoggerSettings.Instance.WarnBackgroundColor;

                break;
            case BadLogType.Error:
                BadConsole.ForegroundColor = BadLoggerSettings.Instance.ErrorForegroundColor;
                BadConsole.BackgroundColor = BadLoggerSettings.Instance.ErrorBackgroundColor;

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        BadConsole.WriteLine(log.ToString());
        BadConsole.ForegroundColor = fg;
        BadConsole.BackgroundColor = bg;
    }
}