using System.Collections.Generic;

using BadScript2.ConsoleAbstraction;
using BadScript2.Debugging;

namespace BadScript2.Debugger;

/// <summary>
///     Implements a Simple Console Debugger
/// </summary>
public class BadConsoleDebugger : IBadDebugger
{
    /// <summary>
    ///     The List of Ignored Files
    /// </summary>
    private static readonly List<string> s_IgnoredFiles = new List<string>();

    /// <summary>
    ///     The last source line that was printed
    /// </summary>
    private int m_LastLine = -1;

    /// <summary>
    ///     The Last source code that was printed
    /// </summary>
    private string? m_LastSource;

	public void Step(BadDebuggerStep stepInfo)
	{
		string view = stepInfo.GetSourceView(out int _, out int lineInSource);

		if (m_LastSource == stepInfo.Position.Source && lineInSource == m_LastLine)
		{
			return;
		}

		m_LastLine = lineInSource;
		m_LastSource = stepInfo.Position.Source;

		if (stepInfo.Position.FileName != null && s_IgnoredFiles.Contains(stepInfo.Position.FileName))
		{
			return;
		}

		BadConsole.WriteLine(view);
		BadConsole.WriteLine("Press any key to continue");

		bool exit = false;

		do
		{
			string cmd = BadConsole.ReadLine()!;

			if (cmd.StartsWith("ignore-file"))
			{
				string file = cmd.Remove(0, "ignore-file".Length).Trim();
				s_IgnoredFiles.Add(file);

				continue;
			}

			if (cmd.StartsWith("file"))
			{
				BadConsole.WriteLine(stepInfo.Position.FileName ?? "NULL");

				continue;
			}

			exit = true;
		}
		while (!exit);
	}
}
