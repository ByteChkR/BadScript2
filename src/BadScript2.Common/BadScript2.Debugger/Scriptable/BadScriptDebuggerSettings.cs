using System;
using System.IO;

using BadScript2.Settings;

namespace BadScript2.Debugger.Scriptable;

public class BadScriptDebuggerSettings : BadSettingsProvider<BadScriptDebuggerSettings>
{
	private BadSettings? m_DebuggerPathObj;

	public BadScriptDebuggerSettings() : base("Runtime.Debugger") { }

	public BadSettings? DebuggerPathObj
	{
		get
		{
			if (m_DebuggerPathObj == null && Settings != null && Settings.HasProperty("Path"))
			{
				m_DebuggerPathObj = Settings?.GetProperty("Path");
			}

			return m_DebuggerPathObj;
		}
	}

	public string? DebuggerPath =>
		DebuggerPathObj?.GetValue<string>() ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debugger.bs");
}
