using BadScript2.Settings;

namespace BadScript2.Runtime.Settings;

/// <summary>
///     Provides settings for the module system.
/// </summary>
public class BadModuleSettings : BadSettingsProvider<BadModuleSettings>
{
    /// <summary>
    ///     Editable Setting for the Setting AllowImportHandlerNullReturn
    /// </summary>
    private readonly BadEditableSetting<BadModuleSettings, bool> m_AllowImportHandlerNullReturn =
        new BadEditableSetting<BadModuleSettings, bool>(nameof(AllowImportHandlerNullReturn));

    /// <summary>
    ///     Editable Setting for the Setting ThrowOnModuleUnresolved
    /// </summary>
    private readonly BadEditableSetting<BadModuleSettings, bool> m_ThrowOnModuleUnresolved =
        new BadEditableSetting<BadModuleSettings, bool>(nameof(ThrowOnModuleUnresolved), true);

    /// <summary>
    ///     Editable Setting for the Setting UseModuleCaching
    /// </summary>
    private readonly BadEditableSetting<BadModuleSettings, bool> m_UseModuleCaching =
        new BadEditableSetting<BadModuleSettings, bool>(nameof(UseModuleCaching), true);

    /// <summary>
    ///     Creates a new instance of the BadModuleSettings class.
    /// </summary>
    public BadModuleSettings() : base("Runtime.Module") { }

    /// <summary>
    ///     If true, the runtime will throw an exception if a module cannot be resolved.
    /// </summary>
    public bool ThrowOnModuleUnresolved
    {
        get => m_ThrowOnModuleUnresolved.GetValue();
        set => m_ThrowOnModuleUnresolved.Set(value);
    }

    /// <summary>
    ///     If true, the module system will cache resolved modules.
    ///     (If this is disabled, the module system will resolve the module every time it is requested, i.e. the modules will
    ///     not have a persistent state across different script executions)
    ///     Recommended value: true
    /// </summary>
    public bool UseModuleCaching
    {
        get => m_UseModuleCaching.GetValue();
        set => m_UseModuleCaching.Set(value);
    }

    /// <summary>
    ///     If true, the runtime will allow import handlers to return null when the import handler claimed that it can handle
    ///     the import request.
    ///     If false, the runtime will continue to search for the package in other import handlers if an import handler
    ///     returned null(after it claimed that it can handle the import request).
    /// </summary>
    public bool AllowImportHandlerNullReturn
    {
        get => m_AllowImportHandlerNullReturn.GetValue();
        set => m_AllowImportHandlerNullReturn.Set(value);
    }
}