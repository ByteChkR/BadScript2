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
	[Value(0, Required = false, HelpText = "The name of the setting")]
    public string Path { get; set; } = "";

    [Option('e',
               "edit",
               Required = false,
               HelpText = "Opens the Default Editor for the Source File associated with the Setting."
           )]
    public bool Edit { get; set; } = false;
}