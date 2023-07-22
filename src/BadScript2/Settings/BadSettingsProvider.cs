using BadScript2.Runtime.Error;

namespace BadScript2.Settings;

/// <summary>
///     Helper class that can be used to automatically load a settings object from a file
/// </summary>
public static class BadSettingsProvider
{
    private static BadSettings? s_RootSettings;

    /// <summary>
    ///     Returns true if the root setting has been set
    /// </summary>
    public static bool HasRootSettings => s_RootSettings != null;

    /// <summary>
    ///     Returns the Root Settings Object
    /// </summary>
    /// <exception cref="BadRuntimeException">Gets raised if the Root Settings Object has not been set.</exception>
    public static BadSettings RootSettings =>
        s_RootSettings ??
        throw new BadRuntimeException("BadSettingsProvider.RootSettings is not initialized");

    /// <summary>
    ///     Sets the Root Settings Object
    /// </summary>
    /// <param name="settings">Root Settings</param>
    public static void SetRootSettings(BadSettings settings)
    {
        s_RootSettings = settings;
    }
}

/// <summary>
///     Helper class that can be used to automatically load a settings object from a file
/// </summary>
public abstract class BadSettingsProvider<T> where T : BadSettingsProvider<T>, new()
{
	/// <summary>
	///     The Instance of the Settings Provider
	/// </summary>
	private static T? s_Instance;

	/// <summary>
	///     The Settings Path Name('.' separated)
	/// </summary>
	private readonly string m_Path;

	/// <summary>
	///     Creates a new Settings Provider
	/// </summary>
	/// <param name="path">The Settings Path</param>
	protected BadSettingsProvider(string path)
    {
        m_Path = path;
    }

	/// <summary>
	///     Returns the Instance of the Settings Provider
	/// </summary>
	public BadSettings? Settings =>
        BadSettingsProvider.HasRootSettings ? BadSettingsProvider.RootSettings.FindOrCreateProperty(m_Path) : null;

	/// <summary>
	///     Returns the Instance of the Settings Provider
	/// </summary>
	public static T Instance => s_Instance ??= new T();
}