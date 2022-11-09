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
    private BadSettings? FileExtensionObj
    {
        get
        {
            if (m_FileExtensionObj == null && Settings != null && Settings.HasProperty(nameof(FileExtension)))
            {
                m_FileExtensionObj = Settings?.GetProperty(nameof(FileExtension));
            }

            return m_FileExtensionObj;
        }
    }

    private BadSettings? m_WriteStackTraceInRuntimeErrorsObj;
    private BadSettings? WriteStackTraceInRuntimeErrorsObj
    {
        get
        {
            if (m_WriteStackTraceInRuntimeErrorsObj == null && Settings != null && Settings.HasProperty(nameof(WriteStackTraceInRuntimeErrors)))
            {
                m_WriteStackTraceInRuntimeErrorsObj = Settings?.GetProperty(nameof(WriteStackTraceInRuntimeErrors));
            }

            return m_WriteStackTraceInRuntimeErrorsObj;
        }
    }
    
    public bool WriteStackTraceInRuntimeErrors => WriteStackTraceInRuntimeErrorsObj?.GetValue<bool>() ?? true;

    /// <summary>
    ///     The Default File Extension of BadScript2 Scripts
    /// </summary>
    public string FileExtension => FileExtensionObj?.GetValue<string>() ?? "bs";
}