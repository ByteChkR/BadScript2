using System;
using System.IO;

using BadScript2.Settings;

namespace BadScript2.Debugger.Scriptable;

/// <summary>
///     The Debugger Settings that are used by the Scriptable Debugger
/// </summary>
public class BadScriptDebuggerSettings : BadSettingsProvider<BadScriptDebuggerSettings>
{
	/// <summary>
	///     The File Path to the Debugger
	/// </summary>
	private readonly BadEditableSetting<BadScriptDebuggerSettings, string> m_DebuggerPath =
		new BadEditableSetting<BadScriptDebuggerSettings, string>("Path",
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "subsystems", "run", "Debugger.bs"));

	/// <summary>
	///     Constructs a new BadScriptDebuggerSettings instance
	/// </summary>
	public BadScriptDebuggerSettings() : base("Runtime.Debugger") { }

	/// <summary>
	///     The File Path to the Debugger
	/// </summary>
	public string DebuggerPath
	{
		get => m_DebuggerPath.GetValue()!;
		set => m_DebuggerPath.Set(value);
	}
}
