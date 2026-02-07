using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Html;

/// <summary>
///     Settings for the Html Template Engine System
/// </summary>
public class BadHtmlSystemSettings
{
	/// <summary>
	///     The Template Files to run
	/// </summary>
	[Option('f', "files", Required = false, HelpText = "The files to run.")]
    public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

	/// <summary>
	///     The Model that the templates will use
	/// </summary>
	[Option("model", Required = false, HelpText = "The Model that the templates will use")]
    public string Model { get; set; } = string.Empty;

	/// <summary>
	///     If Enabled, the Debugger will be attached to the process
	/// </summary>
	[Option('d', "debug", Required = false, HelpText = "Set flag to Attach a Debugger.")]
    public bool Debug { get; set; }

	/// <summary>
	///     If Specified the Remote Console will be started on the specified port
	/// </summary>
	[Option('r',
		       "remote",
		       Required = false,
		       HelpText = "Specifies the Remote Console Host port. If not specified the remote host will not be started"
	       )]
    public int RemotePort { get; set; } = -1;

	/// <summary>
	///     Indicates if empty HTML Text nodes should be skipped
	/// </summary>
	[Option("skipEmptyNodes", Required = false, HelpText = "If enabled, empty text nodes will be skipped.")]
    public bool SkipEmptyTextNodes { get; set; } = false;

	/// <summary>
	///     Indicates if the output should be minified
	/// </summary>
	[Option('m', "minify", Required = false, HelpText = "If enabled, the output will be minified.")]
    public bool Minify { get; set; } = false;
	
	/// <summary>
	///     Sets the handling of Html Comment Nodes, if set to Skip, comment nodes will be treated as normal nodes but will not be included in the output, if set to Execute, comment nodes will be treated as code and will be executed by the template engine, their output will be included in the output·
	/// </summary>
	[Option("comments", Required = false, HelpText = "Sets the handling of Html Comment Nodes, if set to Skip, comment nodes will be treated as normal nodes but will not be included in the output, if set to Execute, comment nodes will be treated as code and will be executed by the template engine, their output will be included in the output")]
	public string CommentMode { get; set; } = "include";
}