using BadHtml;

using BadScript2.Debugger.Scriptable;
using BadScript2.Debugging;
using BadScript2.IO;
using BadScript2.Runtime;

namespace BadScript2.ConsoleCore.Systems.Html;

public class BadHtmlSystem : BadConsoleSystem<BadHtmlSystemSettings>
{
    public override string Name => "html";

    

    public override object? Parse(string[] args)
    {
        if (args.Length == 1)
        {
            return new BadHtmlSystemSettings
            {
                Files = new[] { args[0] },
            };
        }

        return base.Parse(args);
    }

    protected override int Run(BadHtmlSystemSettings settings)
    {
        BadHtmlTemplate.SkipInitialization();

        

        if (settings.Debug)
        {
            BadDebugger.Attach(new BadScriptDebugger(BadExecutionContextOptions.Default));
        }

        foreach (string file in settings.Files)
        {
            string outFile = Path.ChangeExtension(file, "html");
            string html = BadHtmlTemplate.Run(file);
            BadFileSystem.WriteAllText(outFile, html);
        }

        return -1;
    }
}