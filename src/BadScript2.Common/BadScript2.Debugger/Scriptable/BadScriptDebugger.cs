using System;
using System.Collections.Generic;

using BadScript2.Debugging;

/// <summary>
/// Contains the scriptable debugger implementation for the BadScript2 Runtime
/// </summary>
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
	private readonly BadRuntime m_Runtime;

	/// <summary>
	///     The Files that are already seen by the Debugger
	/// </summary>
	private readonly List<string> m_SeenFiles = new List<string>();


	/// <summary>
	///     Constructs a new BadScriptDebugger instance
	/// </summary>
	/// <param name="runtime">The Runtime that the debugger will be loaded into.</param>
	/// <param name="path">The File Path to the Debugger</param>
	/// <param name="source">The Source Code of the Debugger</param>
	public BadScriptDebugger(BadRuntime runtime, string path, string source)
    {
        m_Runtime = runtime
                    .UseApi(new BadScriptDebuggerApi(this, path));

        LoadDebugger(path, source);
    }

#region IBadDebugger Members

    /// <inheritdoc />
    public void Step(BadDebuggerStep stepInfo)
    {
        stepInfo.GetLines(4, 4, out _, out int lineInSource);

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

#endregion

	/// <summary>
	///     Loads the Debugger from the given File Path
	/// </summary>
	/// <param name="path">The File Path</param>
	private void LoadDebugger(string path, string source)
	{
		m_Runtime.Execute(source, path);
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