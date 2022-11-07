using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Provides runtime settings.
/// </summary>
public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{
    /// <summary>
    ///     The File Extension Settings Object
    /// </summary>
    private BadSettings? m_FileExtensionObj;

    /// <summary>
    ///     Creates a new instance of the BadRuntimeSettings class.
    /// </summary>
    public BadRuntimeSettings() : base("Runtime") { }

    /// <summary>
    ///     The File Extension Settings Object
    /// </summary>
    private BadSettings? FileExtensionObj =>
        m_FileExtensionObj ?? (Settings?.HasProperty(nameof(FileExtension)) ?? false ? m_FileExtensionObj ??= Settings?.GetProperty(nameof(FileExtension)) : null);

    /// <summary>
    ///     The Default File Extension of BadScript2 Scripts
    /// </summary>
    public string FileExtension => FileExtensionObj?.GetValue<string>() ?? "bs";
}