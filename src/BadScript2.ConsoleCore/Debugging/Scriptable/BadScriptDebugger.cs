using BadScript2.Common.Logging;
using BadScript2.Debugging;
using BadScript2.Parser;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime;

namespace BadScript2.ConsoleCore.Debugging.Scriptable;

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

    public void Step(BadDebuggerStep stepInfo)
    {
        stepInfo.GetSourceView(out int _, out int lineInSource);

        if (m_LineNumbers.ContainsKey(stepInfo.Position.Source) && lineInSource == m_LineNumbers[stepInfo.Position.Source])
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

    public event Action<BadDebuggerStep>? OnStep;
    public event Action<string>? OnFileLoaded;
}