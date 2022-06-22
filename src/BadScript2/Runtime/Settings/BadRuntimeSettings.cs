using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{

    private BadSettings? m_FileExtensionObj;

    private BadSettings? FileExtensionObj =>
        m_FileExtensionObj ??= Settings?.GetProperty(nameof(FileExtension));
    public string FileExtension => FileExtensionObj?.GetValue<string>() ?? "bs";
    public BadRuntimeSettings() : base("Runtime") { }
}