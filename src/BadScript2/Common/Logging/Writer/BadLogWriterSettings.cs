using BadScript2.Settings;

using Newtonsoft.Json.Linq;

namespace BadScript2.Common.Logging.Writer
{
    /// <summary>
    ///     Contains settings for all log writers
    /// </summary>
    public class BadLogWriterSettings : BadSettingsProvider<BadLogWriterSettings>
    {
        private BadSettings? m_MaskObj;
        public BadLogWriterSettings() : base("Logging.Writer") { }
        private BadSettings? MaskObj => m_MaskObj ??= Settings?.GetProperty(nameof(Mask)) ?? new BadSettings(JToken.FromObject(BadLogMask.All.GetNames()));

        public BadLogMask Mask
        {
            get
            {
                string[]? masks = MaskObj?.GetValue<string[]>();
                if (masks == null)
                {
                    return BadLogMask.None;
                }

                return BadLogMask.GetMask(masks.Select(x => (BadLogMask)x).ToArray());
            }
            set => MaskObj?.SetValue(JToken.FromObject(value.GetNames()));
        }
    }
}