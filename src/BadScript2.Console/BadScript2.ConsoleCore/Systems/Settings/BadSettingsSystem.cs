using System.Diagnostics;

using BadScript2.ConsoleAbstraction;
using BadScript2.Settings;

/// <summary>
/// Contains the 'settings' console command implementation
/// </summary>
namespace BadScript2.ConsoleCore.Systems.Settings;

/// <summary>
///     Prints a specific setting to the console
/// </summary>
public class BadSettingsSystem : BadConsoleSystem<BadSettingsSystemSettings>
{
    /// <summary>
    ///     Creates a new BadSettingsSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadSettingsSystem(BadRuntime runtime) : base(runtime) { }

    /// <inheritdoc />
    public override string Name => "settings";

    private void OpenWithDefault(string path)
    {
        ProcessStartInfo pi = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        };
        Process.Start(pi);
    }
    /// <inheritdoc />
    protected override Task<int> Run(BadSettingsSystemSettings settings)
    {
        BadSettings? setting = string.IsNullOrEmpty(settings.Path) ? BadSettingsProvider.RootSettings : BadSettingsProvider.RootSettings.FindProperty(settings.Path);

        if(settings.Edit)
        {
            if (BadSettingsProvider.RootSettings == setting)
            {
                //Open the Settings folder
                //Directory: $Console.DataDirectory/settings
                OpenWithDefault(Path.GetFullPath(BadSettingsProvider.RootSettings.FindProperty<string>("Console.DataDirectory")! + "/settings"));
                return Task.FromResult(0);
            }

            if (setting == null)
            {
                BadConsole.WriteLine($"Setting '{settings.Path}' not found.");
                return Task.FromResult(1);
            }

            if (!setting.HasSourcePath)
            {
                BadConsole.WriteLine($"Setting '{settings.Path}' has no source path.");
                return Task.FromResult(1);
            }
            
            //Open the source file in the default editor
            string path = Path.GetFullPath(setting.SourcePath);
            OpenWithDefault(path);
            
            return Task.FromResult(0);
        }
        BadConsole.WriteLine(setting?.ToString() ?? "null");

        return Task.FromResult(0);
    }
}