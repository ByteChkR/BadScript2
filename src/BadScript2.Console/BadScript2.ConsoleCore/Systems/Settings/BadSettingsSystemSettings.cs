using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Settings;

/// <summary>
///     Settings for the Settings System
/// </summary>
public class BadSettingsSystemSettings
{
	/// <summary>
	///     Name of the Settings to Display
	/// </summary>
	[Value(0, Required = true, HelpText = "The name of the setting")]
    public string Path { get; set; } = "";
}