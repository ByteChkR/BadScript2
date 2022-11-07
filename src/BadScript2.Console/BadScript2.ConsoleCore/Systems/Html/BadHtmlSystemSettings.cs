using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Html;

public class BadHtmlSystemSettings
{
    [Option('f', "files", Required = false, HelpText = "The files to run.")]
    public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

    [Option('d', "debug", Required = false, HelpText = "Set flag to Attach a Debugger.")]
    public bool Debug { get; set; }

    [Option('r', "remote", Required = false, HelpText = "Specifies the Remote Console Host port. If not specified the remote host will not be started")]
    public int RemotePort { get; set; } = -1;
}