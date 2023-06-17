using BadScript2.Settings;

using Newtonsoft.Json.Linq;

namespace BadScript2.Common.Logging.Writer;

/// <summary>
///     Contains settings for all log writers
/// </summary>
public class BadLogWriterSettings : BadSettingsProvider<BadLogWriterSettings>
{

	public BadLogWriterSettings() : base("Logging.Writer") { }


	
	private readonly BadEditableSetting<BadLogWriterSettings, BadLogMask> m_Mask =
		new BadEditableSetting<BadLogWriterSettings, BadLogMask>(nameof(Mask), BadLogMask.All);
	public BadLogMask Mask
	{
		get => m_Mask.GetValue()!;
		set => m_Mask.Set(value);
	}
}
