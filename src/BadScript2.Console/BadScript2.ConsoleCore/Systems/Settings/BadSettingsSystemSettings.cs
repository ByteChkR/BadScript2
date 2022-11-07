using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Settings;

public class BadSettingsSystemSettings
{
    [Value(0, Required = true, HelpText = "The name of the setting")]
    public string Path { get; set; } = "";
}