using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Html;

public class BadHtmlSystemSettings
{
    [Option('f', "files", Required = false, HelpText = "The files to run.")]
    public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

    [Option('d', "debug", Required = false, HelpText = "Set flag to Attach a Debugger.")]
    public bool Debug { get; set; }
}