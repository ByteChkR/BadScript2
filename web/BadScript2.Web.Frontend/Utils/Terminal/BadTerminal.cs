namespace BadScript2.Web.Frontend.Utils;

public class BadTerminal
{
    public readonly BadReplContext Context;

    public string TerminalPrefix { get; set; } = "$";

    private readonly List<BadTerminalCommand> m_Commands = new List<BadTerminalCommand>();

    public bool IsRunning { get; private set; }

    private readonly CancellationTokenSource m_Cts = new CancellationTokenSource();
    public BadTerminal(BadReplContext context)
    {
        Context = context;
        RegisterCommand(new BadChangeDirectoryTerminalCommand());
        RegisterCommand(new BadListDirectoryTerminalCommand());
        RegisterCommand(new BadOpenFileTerminalCommand());
        RegisterCommand(new BadScriptConsoleTerminalCommand());
        RegisterCommand(new BadClearConsoleTerminalCommand());
    }
    public void RegisterCommand(BadTerminalCommand cmd) => m_Commands.Add(cmd);
    public void Start()
    {
        if (IsRunning) return;
        IsRunning = true;
        Task.Run(() => Loop(m_Cts.Token));
    }

    public void Stop()
    {
        if (!IsRunning) return;
        m_Cts.Cancel();
    }

    private void WritePrefix()
    {
        Context.Console.Write($"{Context.FileSystem.GetCurrentDirectory()} {TerminalPrefix} ");
    }
    private async Task Loop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            WritePrefix();
            string input = await Context.Console.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(input)) continue;
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string cmd = parts[0];
            string[] args = parts.Skip(1).ToArray();
            BadTerminalCommand? command = m_Commands.FirstOrDefault(c => c.Names.Contains(cmd));
            if (command == null)
            {
                Context.Console.WriteLine($"Command '{cmd}' not found.");
                continue;
            }

            try
            {
                await command.Run(Context, args);
            }
            catch (Exception e)
            {
                Context.Console.WriteLine($"Error: {e.Message}");
            }
        }

        IsRunning = false;
    }
}