using BadScript2.Debugging;

namespace BadScript2.ConsoleCore.Debugging;

public class BadConsoleDebugger : IBadDebugger
{
    private static readonly List<string> s_IgnoredFiles = new List<string>();
    private int m_LastLine = -1;
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

        Console.WriteLine(view);
        Console.WriteLine("Press any key to continue");

        bool exit = false;
        do
        {
            string cmd = Console.ReadLine()!;
            if (cmd.StartsWith("ignore-file"))
            {
                string file = cmd.Remove(0, "ignore-file".Length).Trim();
                s_IgnoredFiles.Add(file);

                continue;
            }

            if (cmd.StartsWith("file"))
            {
                Console.WriteLine(stepInfo.Position.FileName ?? "NULL");

                continue;
            }

            exit = true;
        }
        while (!exit);
    }
}