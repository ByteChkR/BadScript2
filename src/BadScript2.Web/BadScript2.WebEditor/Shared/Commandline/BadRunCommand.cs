namespace BadScript2.WebEditor.Shared.Commandline;

public class BadRunCommand : BadConsoleCommand
{
    private readonly Action<string> m_RunScript;

    public BadRunCommand(Action<string> commandFunc) : base(
        "run",
        "Runs the specifies BadScript File",
        new[]
        {
            "exec",
            "r",
        },
        new[]
        {
            "script-file-path",
        }
    )
    {
        m_RunScript = commandFunc;
    }

    public override string Execute(string args)
    {
        m_RunScript(args);

        return "";
    }
}