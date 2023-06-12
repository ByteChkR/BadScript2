using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Provides runtime settings.
/// </summary>
public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{
	private BadSettings? m_CatchRuntimeExceptionsObj;

	/// <summary>
	///     The File Extension Settings Object
	/// </summary>
	private BadSettings? m_FileExtensionObj;

	private BadSettings? m_WriteStackTraceInRuntimeErrorsObj;

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

	private BadSettings? WriteStackTraceInRuntimeErrorsObj
	{
		get
		{
			if (m_WriteStackTraceInRuntimeErrorsObj == null &&
			    Settings != null &&
			    Settings.HasProperty(nameof(WriteStackTraceInRuntimeErrors)))
			{
				m_WriteStackTraceInRuntimeErrorsObj = Settings?.GetProperty(nameof(WriteStackTraceInRuntimeErrors));
			}

			return m_WriteStackTraceInRuntimeErrorsObj;
		}
	}

	private BadSettings? CatchRuntimeExceptionsObj
	{
		get
		{
			if (m_CatchRuntimeExceptionsObj == null &&
			    Settings != null &&
			    Settings.HasProperty(nameof(CatchRuntimeExceptions)))
			{
				m_CatchRuntimeExceptionsObj = Settings?.GetProperty(nameof(CatchRuntimeExceptions));
			}

			return m_CatchRuntimeExceptionsObj;
		}
	}

	public bool WriteStackTraceInRuntimeErrors
	{
		get => WriteStackTraceInRuntimeErrorsObj?.GetValue<bool>() ?? false;
		set
		{
			if (WriteStackTraceInRuntimeErrorsObj == null)
			{
				m_WriteStackTraceInRuntimeErrorsObj = new BadSettings();
			}

			m_WriteStackTraceInRuntimeErrorsObj!.SetValue(value);
		}
	}

	/// <summary>
	///     The Default File Extension of BadScript2 Scripts
	/// </summary>
	public string FileExtension => FileExtensionObj?.GetValue<string>() ?? "bs";

	public bool CatchRuntimeExceptions
	{
		get => CatchRuntimeExceptionsObj?.GetValue<bool>() ?? true;
		set
		{
			if (CatchRuntimeExceptionsObj == null)
			{
				m_CatchRuntimeExceptionsObj = new BadSettings();
			}

			m_CatchRuntimeExceptionsObj!.SetValue(value);
		}
	}
}
