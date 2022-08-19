using BadScript2.Settings;

namespace BadScript2.ConsoleCore.Systems.Settings;

public class BadSettingsSystem : BadConsoleSystem<BadSettingsSystemSettings>
{
    public override string Name => "settings";

    protected override int Run(BadSettingsSystemSettings settings)
    {
        BadSettings? setting = BadSettingsProvider.RootSettings.FindProperty(settings.Path);
        Console.WriteLine(
            setting?.ToString() ?? "null"
        );

        return -1;
    }
}