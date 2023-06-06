using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Html;


/// <summary>
/// Settings for the Html Template Engine System
/// </summary>
public class BadHtmlSystemSettings
{
	/// <summary>
	/// The Template Files to run
	/// </summary>
	[Option('f', "files", Required = false, HelpText = "The files to run.")]
	public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

	/// <summary>
	/// If Enabled, the Debugger will be attached to the process
	/// </summary>
	[Option('d', "debug", Required = false, HelpText = "Set flag to Attach a Debugger.")]
	public bool Debug { get; set; }

	/// <summary>
	/// If Specified the Remote Console will be started on the specified port
	/// </summary>
	[Option('r',
		"remote",
		Required = false,
		HelpText = "Specifies the Remote Console Host port. If not specified the remote host will not be started")]
	public int RemotePort { get; set; } = -1;
}
