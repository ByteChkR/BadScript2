using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Shell;

public class BadShellSystemSettings
{
	[Option('c', "command", HelpText = "Execute a single command and exit")]
	public string? Command { get; set; }
}
