using BadScript2.Settings;

namespace BadScript2.Common.Logging.Writer;

/// <summary>
///     Contains settings for all log writers
/// </summary>
public class BadLogWriterSettings : BadSettingsProvider<BadLogWriterSettings>
{
    /// <summary>
    /// Backing Field of <see cref="Mask"/>
    /// </summary>
    private readonly BadEditableSetting<BadLogWriterSettings, string[]> m_Mask =
        new BadEditableSetting<BadLogWriterSettings, string[]>(nameof(Mask), BadLogMask.All.GetNames());

    /// <summary>
    /// Creates a new instance of the Logger Settings
    /// </summary>
    public BadLogWriterSettings() : base("Logging.Writer") { }

    /// <summary>
    /// The mask of the log writer
    /// </summary>
    public BadLogMask Mask
    {
        get => BadLogMask.GetMask(m_Mask.GetValue()!.Select(x => (BadLogMask)x)
                                        .ToArray()
                                 );
        set => m_Mask.Set(value.GetNames());
    }
}