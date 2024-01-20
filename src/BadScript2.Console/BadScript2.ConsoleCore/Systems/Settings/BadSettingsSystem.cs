using BadScript2.ConsoleAbstraction;
using BadScript2.Settings;

namespace BadScript2.ConsoleCore.Systems.Settings;

/// <summary>
///     Prints a specific setting to the console
/// </summary>
public class BadSettingsSystem : BadConsoleSystem<BadSettingsSystemSettings>
{
    public BadSettingsSystem(BadRuntime runtime) : base(runtime) { }

    public override string Name => "settings";

    protected override int Run(BadSettingsSystemSettings settings)
    {
        BadSettings? setting = string.IsNullOrEmpty(settings.Path) ? BadSettingsProvider.RootSettings : BadSettingsProvider.RootSettings.FindProperty(settings.Path);

        BadConsole.WriteLine(setting?.ToString() ?? "null");

        return 0;
    }
}