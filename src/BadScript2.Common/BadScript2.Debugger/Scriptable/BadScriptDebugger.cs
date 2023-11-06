using System;
using System.Collections.Generic;

using BadScript2.Debugging;
using BadScript2.Parser;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime;

namespace BadScript2.Debugger.Scriptable;

/// <summary>
///     Implements a Scriptable Debugger
/// </summary>
public class BadScriptDebugger : IBadDebugger
{
	/// <summary>
	///     The Line Numbers/Positions for the Debugger
	/// </summary>
	private readonly Dictionary<string, int> m_LineNumbers = new Dictionary<string, int>();

	/// <summary>
	///     The Debugger Execution Context Options
	/// </summary>
	private readonly BadExecutionContextOptions m_Options;

	/// <summary>
	///     The Files that are already seen by the Debugger
	/// </summary>
	private readonly List<string> m_SeenFiles = new List<string>();

	/// <summary>
	///     Constructs a new BadScriptDebugger instance
	/// </summary>
	/// <param name="options">The Context Options</param>
	/// <param name="debuggerPath">The File Path to the Debugger</param>
	public BadScriptDebugger(BadExecutionContextOptions options, string debuggerPath)
	{
		m_Options = options;
		m_Options.AddApi(new BadScriptDebuggerApi(this));
		LoadDebugger(debuggerPath);
	}

	/// <summary>
	///     Constructs a new BadScriptDebugger instance
	/// </summary>
	/// <param name="options">The Context Options</param>
	public BadScriptDebugger(BadExecutionContextOptions options)
	{
		m_Options = options;
		m_Options.AddApi(new BadScriptDebuggerApi(this));


		LoadDebugger(BadScriptDebuggerSettings.Instance.DebuggerPath);
	}

	public void Step(BadDebuggerStep stepInfo)
	{
		stepInfo.GetSourceView(out int _, out int lineInSource);

		if (m_LineNumbers.ContainsKey(stepInfo.Position.Source) &&
		    lineInSource == m_LineNumbers[stepInfo.Position.Source])
		{
			return;
		}

		m_LineNumbers[stepInfo.Position.Source] = lineInSource;

		if (!m_SeenFiles.Contains(stepInfo.Position.FileName ?? ""))
		{
			m_SeenFiles.Add(stepInfo.Position.FileName ?? "");
			OnFileLoaded?.Invoke(stepInfo.Position.FileName ?? "");
		}

		OnStep?.Invoke(stepInfo);
	}

	/// <summary>
	///     Loads the Debugger from the given File Path
	/// </summary>
	/// <param name="path">The File Path</param>
	private void LoadDebugger(string path)
	{
		BadExecutionContext ctx = m_Options.Build();
		ctx.Run(new BadSourceParser(BadSourceReader.FromFile(path),
			BadOperatorTable.Instance).Parse());
	}

	/// <summary>
	///     Event that gets called on every debugger step
	/// </summary>
	public event Action<BadDebuggerStep>? OnStep;

	/// <summary>
	///     Event that gets called when a new file is loaded.
	/// </summary>
	public event Action<string>? OnFileLoaded;
}
