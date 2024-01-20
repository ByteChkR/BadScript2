using BadScript2.ConsoleAbstraction;
using BadScript2.Settings;

namespace BadScript2.ConsoleCore.Systems.Settings;

/// <summary>
///     Prints a specific setting to the console
/// </summary>
public class BadSettingsSystem : BadConsoleSystem<BadSettingsSystemSettings>
{
    /// <summary>
    /// Creates a new BadSettingsSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadSettingsSystem(BadRuntime runtime) : base(runtime) { }

    /// <inheritdoc/>
    public override string Name => "settings";

    /// <inheritdoc/>
    protected override int Run(BadSettingsSystemSettings settings)
    {
        BadSettings? setting = string.IsNullOrEmpty(settings.Path) ? BadSettingsProvider.RootSettings : BadSettingsProvider.RootSettings.FindProperty(settings.Path);

        BadConsole.WriteLine(setting?.ToString() ?? "null");

        return 0;
    }
}