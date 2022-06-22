using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{

    private BadSettings? m_DefaultExtensionObj;

    private BadSettings? DefaultExtensionObj =>
        m_DefaultExtensionObj ??= Settings?.GetProperty(nameof(DefaultExtension));
    public string DefaultExtension => DefaultExtensionObj?.GetValue<string>() ?? "bs";
    public BadRuntimeSettings() : base("Runtime") { }
}