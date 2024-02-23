using BadScript2.IO;

/// <summary>
/// Contains the Core Functionality of the Console Application
/// </summary>
namespace BadScript2.ConsoleCore;

/// <summary>
///     Static class that contains all the directories used by the console
/// </summary>
public static class BadConsoleDirectories
{
	/// <summary>
	///     The Data Directory
	/// </summary>
	public static string DataDirectory
    {
        get
        {
            string s = Path.Combine(BadFileSystem.Instance.GetStartupDirectory(), "data");

            BadFileSystem.Instance.CreateDirectory(s);

            return s;
        }
    }

	/// <summary>
	///     The Log File
	/// </summary>
	public static string LogFile => Path.Combine(DataDirectory, "logs.log");
}