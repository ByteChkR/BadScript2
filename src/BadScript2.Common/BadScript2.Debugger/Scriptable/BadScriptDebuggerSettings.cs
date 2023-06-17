using System;
using System.IO;

using BadScript2.Settings;

namespace BadScript2.Debugger.Scriptable;

public class BadScriptDebuggerSettings : BadSettingsProvider<BadScriptDebuggerSettings>
{
	public BadScriptDebuggerSettings() : base("Runtime.Debugger") { }

	
	private BadEditableSetting<BadScriptDebuggerSettings, string> m_DebuggerPath =
		new BadEditableSetting<BadScriptDebuggerSettings, string>(nameof(DebuggerPath), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debugger.bs"));
	
	public string DebuggerPath
	{
		get => m_DebuggerPath.GetValue()!;
		set => m_DebuggerPath.Set(value);
	}
}
