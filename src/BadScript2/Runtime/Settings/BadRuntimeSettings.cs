using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

public class BadRuntimeSettings : BadSettingsProvider<BadRuntimeSettings>
{
    private BadSettings? m_FileExtensionObj;
    public BadRuntimeSettings() : base("Runtime") { }

    private BadSettings? FileExtensionObj =>
        m_FileExtensionObj ??= Settings?.GetProperty(nameof(FileExtension));

    public string FileExtension => FileExtensionObj?.GetValue<string>() ?? "bs";
}