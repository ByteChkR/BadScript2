using BadScript2.Common.Logging;
using BadScript2.Debugging;
using BadScript2.Parser;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime;

namespace BadScript2.Console.Debugging.Scriptable;

public class BadScriptDebugger : IBadDebugger
{
    private readonly Dictionary<string, int> m_LineNumbers = new Dictionary<string, int>();
    private readonly BadExecutionContextOptions m_Options;
    private readonly List<string> m_SeenFiles = new List<string>();

    public BadScriptDebugger(BadExecutionContextOptions options)
    {
        m_Options = options;
        m_Options.Apis.Add(new BadScriptDebuggerApi(this));

        if (BadScriptDebuggerSettings.Instance.DebuggerPath == null)
        {
            BadLogger.Warn("Debugger path not set, debugger will not be available", "Debugger");
        }
        else
        {
            BadExecutionContext ctx = m_Options.Build();
            ctx.Run(
                new BadSourceParser(
                    BadSourceReader.FromFile(BadScriptDebuggerSettings.Instance.DebuggerPath),
                    BadOperatorTable.Instance
                ).Parse()
            );
        }
    }

    public void Step(BadDebuggerStep step)
    {
        step.GetSourceView(out int _, out int lineInSource);

        if (m_LineNumbers.ContainsKey(step.Position.Source) && lineInSource == m_LineNumbers[step.Position.Source])
        {
            return;
        }

        m_LineNumbers[step.Position.Source] = lineInSource;
        if (!m_SeenFiles.Contains(step.Position.FileName ?? ""))
        {
            m_SeenFiles.Add(step.Position.FileName ?? "");
            OnFileLoaded?.Invoke(step.Position.FileName ?? "");
        }

        OnStep?.Invoke(step);
    }

    public event Action<BadDebuggerStep>? OnStep;
    public event Action<string>? OnFileLoaded;
}