using BadScript2.Runtime.Error;

namespace BadScript2.Settings;

/// <summary>
/// Helper class that can be used to automatically load a settings object from a file
/// </summary>
public static class BadSettingsProvider
{
    private const string SETTINGS_FILE = "Settings.json";


    private static BadSettings? s_RootSettings;

    public static bool HasRootSettings => s_RootSettings != null;

    public static BadSettings RootSettings => s_RootSettings ??
                                              throw new BadRuntimeException(
                                                  "BadSettingsProvider.RootSettings is not initialized"
                                              );

    public static void SetRootSettings(BadSettings settings)
    {
        s_RootSettings = settings;
    }
}

/// <summary>
/// Helper class that can be used to automatically load a settings object from a file
/// </summary>
public abstract class BadSettingsProvider<T> where T : BadSettingsProvider<T>, new()
{
    private static T? s_Instance;
    private readonly string m_Path;

    protected BadSettingsProvider(string path)
    {
        m_Path = path;
    }

    protected BadSettings? Settings => BadSettingsProvider.HasRootSettings
        ? BadSettingsProvider.RootSettings.FindProperty(m_Path)
        : null;

    public static T Instance => s_Instance ??= new T();
}