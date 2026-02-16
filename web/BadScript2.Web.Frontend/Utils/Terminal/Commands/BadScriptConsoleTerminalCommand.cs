using BadScript2.ConsoleCore;
using BadScript2.ConsoleCore.Systems.Docs;
using BadScript2.ConsoleCore.Systems.Html;
using BadScript2.ConsoleCore.Systems.Run;
using BadScript2.ConsoleCore.Systems.Settings;
using BadScript2.ConsoleCore.Systems.Test;
namespace BadScript2.Web.Frontend.Utils;

public class BadScriptConsoleTerminalCommand : BadTerminalCommand
{
    public BadScriptConsoleTerminalCommand() : base("The 'bs' commandline.", "bs") { }
    public override Task Run(BadReplContext context, string[] args)
    {
        BadConsoleRunner runner = new BadConsoleRunner(
            new BadDefaultRunSystem(() => context.Runtime),
            new BadTestSystem(() => context.Runtime),
            new BadRunSystem(() => context.Runtime),
            new BadSettingsSystem(() => context.Runtime),
            new BadHtmlSystem(() => context.Runtime),
            new BadRemoteConsoleSystem(() => context.Runtime),
            new BadDocsSystem(() => context.Runtime)
        );
        /*
            new BadDefaultRunSystem(runtime),
            new BadTestSystem(runtime),
            new BadRunSystem(runtime),
            new BadSettingsSystem(runtime),
            new BadHtmlSystem(runtime),
            new BadRemoteConsoleSystem(runtime),
            new BadDocsSystem(runtime)*/
        
        return runner.Run(args);
    }
}