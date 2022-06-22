using BadScript2.Settings;

using Newtonsoft.Json.Linq;

namespace BadScript2.Common.Logging.Writer
{
    public class BadLogWriterSettings : BadSettingsProvider<BadLogWriterSettings>
    {
        public BadLogWriterSettings() : base("Logging.Writer") { }

        private BadSettings? m_MaskObj;
        private BadSettings? MaskObj => m_MaskObj ??= Settings?.GetProperty(nameof(Mask));
    
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
            set => m_MaskObj?.SetValue(JToken.FromObject(value.GetNames()));
        }
    }
}