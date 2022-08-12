using BadScript2.IO;

namespace BadScript2.Console;

public static class BadConsoleDirectories
{
    public static string DataDirectory
    {
        get
        {
            string s = Path.Combine(BadFileSystem.Instance.GetStartupDirectory(), "data");

            BadFileSystem.Instance.CreateDirectory(s);

            return s;
        }
    }

    public static string LogFile => Path.Combine(DataDirectory, "logs.log");
}