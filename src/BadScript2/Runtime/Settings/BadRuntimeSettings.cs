using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Provides runtime settings.
/// </summary>
public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{
	private readonly BadEditableSetting<BadRuntimeSettings, bool> m_CatchRuntimeExceptions =
		new BadEditableSetting<BadRuntimeSettings, bool>(nameof(CatchRuntimeExceptions), true);

	private readonly BadEditableSetting<BadRuntimeSettings, string> m_FileExtension =
		new BadEditableSetting<BadRuntimeSettings, string>(nameof(FileExtension), "bs");


	private readonly BadEditableSetting<BadRuntimeSettings, bool> m_WriteStackTraceInRuntimeErrors =
		new BadEditableSetting<BadRuntimeSettings, bool>(nameof(WriteStackTraceInRuntimeErrors));

    /// <summary>
    ///     Creates a new instance of the BadRuntimeSettings class.
    /// </summary>
    public BadRuntimeSettings() : base("Runtime") { }

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

	public bool CatchRuntimeExceptions
	{
		get => m_CatchRuntimeExceptions.GetValue();
		set => m_CatchRuntimeExceptions.Set(value);
	}
}
