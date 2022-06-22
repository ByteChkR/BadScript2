using CommandLine;

namespace BadScript2.Console.Systems.Run;

public class BadRunSystemSettings
{
    [Option('f', "files", Required = false, HelpText = "The files to run.")]
    public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string>();

    [Option('i', "interactive", Required = false, HelpText = "Run in interactive mode.")]
    public bool Interactive { get; set; }

    [Option('a', "args", Required = false, HelpText = "Arguments to pass to the script.")]
    public IEnumerable<string> Args { get; set; } = Enumerable.Empty<string>();
    
    [Option('b', "benchmark", Required = false, HelpText = "Set flag to Measure Execution Time.")]
    public bool Benchmark { get; set; }
    [Option('d', "debug", Required = false, HelpText = "Set flag to Attach a Debugger.")]
    public bool Debug { get; set; }

}