using System.Net;
using System.Net.Sockets;

using BadHtml;

using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleAbstraction.Implementations.Remote;
using BadScript2.Debugger.Scriptable;
using BadScript2.Debugging;
using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Settings;

namespace BadScript2.ConsoleCore.Systems.Html;

/// <summary>
///     Runs the Html Template Engine
/// </summary>
public class BadHtmlSystem : BadConsoleSystem<BadHtmlSystemSettings>
{
    public override string Name => "html";


    public override object? Parse(string[] args)
    {
        if (args.Length == 1)
        {
            return new BadHtmlSystemSettings
            {
                Files = new[]
                {
                    args[0],
                },
            };
        }

        return base.Parse(args);
    }

    protected override int Run(BadHtmlSystemSettings settings)
    {
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
        BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors = true;


        if (settings.Debug)
        {
            BadDebugger.Attach(new BadScriptDebugger(BadExecutionContextOptions.Default));
        }

        BadNetworkConsoleHost? host = null;

        if (settings.RemotePort != -1)
        {
            host = new BadNetworkConsoleHost(new TcpListener(IPAddress.Any, settings.RemotePort));
            host.Start();
            BadConsole.SetConsole(host);
        }

        foreach (string file in settings.Files)
        {
            string outFile = Path.ChangeExtension(file, "html");
            string html = BadHtmlTemplate.Create(file).Run();
            BadFileSystem.WriteAllText(outFile, html);
        }

        host?.Stop();

        return -1;
    }
}