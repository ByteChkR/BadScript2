using BadScript2.Debugging;

namespace BadScript2.Console.Debugging;

public class BadConsoleDebugger : IBadDebugger
{
    private static readonly List<string> s_IgnoredFiles = new List<string>();
    private int m_LastLine = -1;
    private string? m_LastSource;

    public void Step(BadDebuggerStep step)
    {
        string view = step.GetSourceView(out int _, out int lineInSource);

        if (m_LastSource == step.Position.Source && lineInSource == m_LastLine)
        {
            return;
        }

        m_LastLine = lineInSource;
        m_LastSource = step.Position.Source;
        if (step.Position.FileName != null && s_IgnoredFiles.Contains(step.Position.FileName))
        {
            return;
        }

        System.Console.WriteLine(view);
        System.Console.WriteLine("Press any key to continue");

        bool exit = false;
        do
        {
            string cmd = System.Console.ReadLine()!;
            if (cmd.StartsWith("ignore-file"))
            {
                string file = cmd.Remove(0, "ignore-file".Length).Trim();
                s_IgnoredFiles.Add(file);

                continue;
            }

            if (cmd.StartsWith("file"))
            {
                System.Console.WriteLine(step.Position.FileName ?? "NULL");

                continue;
            }

            exit = true;
        }
        while (!exit);
    }
}