using BadScript2.Settings;

namespace BadScript2.ConsoleCore.Debugging.Scriptable;

public class BadScriptDebuggerSettings : BadSettingsProvider<BadScriptDebuggerSettings>
{
    private BadSettings? m_DebuggerPathObj;
    public BadScriptDebuggerSettings() : base("Runtime.Debugger") { }
    public BadSettings? DebuggerPathObj => m_DebuggerPathObj ?? (Settings?.HasProperty("Path") ?? false ? m_DebuggerPathObj ??= Settings?.GetProperty("Path") : null);
    public string? DebuggerPath => DebuggerPathObj?.GetValue<string>();
}