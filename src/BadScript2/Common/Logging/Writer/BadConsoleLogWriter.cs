namespace BadScript2.Common.Logging.Writer
{
    public class BadConsoleLogWriter : BadLogWriter
    {
        protected override void Write(BadLog log)
        {
            ConsoleColor fg = Console.ForegroundColor;
            ConsoleColor bg = Console.BackgroundColor;
            switch (log.Type)
            {
                case BadLogType.Log:
                    Console.ForegroundColor = BadLoggerSettings.Instance.LogForegroundColor;
                    Console.BackgroundColor = BadLoggerSettings.Instance.LogBackgroundColor;

                    break;
                case BadLogType.Warning:
                    Console.ForegroundColor = BadLoggerSettings.Instance.WarnForegroundColor;
                    Console.BackgroundColor = BadLoggerSettings.Instance.WarnBackgroundColor;

                    break;
                case BadLogType.Error:
                    Console.ForegroundColor = BadLoggerSettings.Instance.ErrorForegroundColor;
                    Console.BackgroundColor = BadLoggerSettings.Instance.ErrorBackgroundColor;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine(log);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }
    }
}