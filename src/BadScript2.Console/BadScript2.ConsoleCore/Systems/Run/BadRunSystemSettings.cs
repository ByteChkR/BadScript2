using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
/// Settings for the Run System
/// </summary>
public class BadRunSystemSettings
{
	/// <summary>
	/// The Files that will be executed
	/// </summary>
	[Option('f', "files", Required = false, HelpText = "The files to run.")]
	public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

	/// <summary>
	/// If Enabled, the Console will be started in interactive mode
	/// </summary>
	[Option('i', "interactive", Required = false, HelpText = "Run in interactive mode.")]
	public bool Interactive { get; set; }

	/// <summary>
	/// The Commandline Arguments for the Scripts
	/// </summary>
	[Option('a', "args", Required = false, HelpText = "Arguments to pass to the script.")]
	public IEnumerable<string> Args { get; set; } = Enumerable.Empty<string>();

	/// <summary>
	/// If Enabled, the execution time will be printed to the console
	/// </summary>
	[Option('b', "benchmark", Required = false, HelpText = "Set flag to Measure Execution Time.")]
	public bool Benchmark { get; set; }

	/// <summary>
	/// If enabled, the debugger will be attached to the process
	/// </summary>
	[Option('d', "debug", Required = false, HelpText = "Set flag to Attach a Debugger.")]
	public bool Debug { get; set; }

	/// <summary>
	/// If specified, the Run System will try to host a remote shell on the specified port
	/// </summary>
	[Option('r',
		"remote",
		Required = false,
		HelpText = "Specifies the Remote Console Host port. If not specified the remote host will not be started")]
	public int RemotePort { get; set; } = -1;
}
