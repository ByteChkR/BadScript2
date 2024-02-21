using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Provides runtime settings.
/// </summary>
public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{
    /// <summary>
    ///     Editable Setting for the Setting CatchRuntimeExceptions
    /// </summary>
    private readonly BadEditableSetting<BadRuntimeSettings, bool> m_CatchRuntimeExceptions =
        new BadEditableSetting<BadRuntimeSettings, bool>(nameof(CatchRuntimeExceptions), true);

    /// <summary>
    ///     Editable Setting for the Setting FileExtension
    /// </summary>
    private readonly BadEditableSetting<BadRuntimeSettings, string> m_FileExtension =
        new BadEditableSetting<BadRuntimeSettings, string>(nameof(FileExtension), "bs");


    /// <summary>
    ///     Editable Setting for the Setting WriteStackTraceInRuntimeErrors
    /// </summary>
    private readonly BadEditableSetting<BadRuntimeSettings, bool> m_WriteStackTraceInRuntimeErrors =
        new BadEditableSetting<BadRuntimeSettings, bool>(nameof(WriteStackTraceInRuntimeErrors));

    /// <summary>
    ///     Creates a new instance of the BadRuntimeSettings class.
    /// </summary>
    public BadRuntimeSettings() : base("Runtime") { }

    /// <summary>
    ///     If true, the runtime will write the stack trace of runtime errors to the console.
    /// </summary>
    public bool WriteStackTraceInRuntimeErrors
    {
        get => m_WriteStackTraceInRuntimeErrors.GetValue();
        set => m_WriteStackTraceInRuntimeErrors.Set(value);
    }

    /// <summary>
    ///     The Default File Extension of BadScript2 Scripts
    /// </summary>
    public string FileExtension
    {
        get => m_FileExtension.GetValue()!;
        set => m_FileExtension.Set(value);
    }

    /// <summary>
    ///     If true, the runtime will catch C# exceptions and expose them as runtime errors.
    /// </summary>
    public bool CatchRuntimeExceptions
    {
        get => m_CatchRuntimeExceptions.GetValue();
        set => m_CatchRuntimeExceptions.Set(value);
    }
}