using System.Net;
using System.Net.Sockets;

using BadHtml;

using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleAbstraction.Implementations.Remote;
using BadScript2.Debugger;
using BadScript2.Interop.Json;
using BadScript2.IO;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;

/// <summary>
/// Contains the 'html' console command implementation
/// </summary>
namespace BadScript2.ConsoleCore.Systems.Html;

/// <summary>
///     Runs the Html Template Engine
/// </summary>
public class BadHtmlSystem : BadConsoleSystem<BadHtmlSystemSettings>
{
    /// <summary>
    ///     Creates a new BadHtmlSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadHtmlSystem(BadRuntime runtime) : base(runtime) { }


    /// <inheritdoc />
    public override string Name => "html";


    /// <inheritdoc />
    public override object? Parse(string[] args)
    {
        if (args.Length == 1 && args[0] != "--help" && args[0] != "help")
        {
            return new BadHtmlSystemSettings { Files = new[] { args[0] } };
        }

        return base.Parse(args);
    }

    /// <inheritdoc />
    protected override Task<int> Run(BadHtmlSystemSettings settings)
    {
        BadRuntimeSettings.Instance.CatchRuntimeExceptions = false;
        BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors = true;

        if (settings.Debug)
        {
            Runtime.UseScriptDebugger();
        }

        BadNetworkConsoleHost? host = null;

        if (settings.RemotePort != -1)
        {
            host = new BadNetworkConsoleHost(new TcpListener(IPAddress.Any, settings.RemotePort));
            host.Start();
            Runtime.UseConsole(host);
        }

        BadHtmlTemplateOptions opts = new BadHtmlTemplateOptions
        {
            Runtime = Runtime, SkipEmptyTextNodes = settings.SkipEmptyTextNodes,
        };

        BadObject? model = null;

        if (!string.IsNullOrEmpty(settings.Model))
        {
            model = BadJson.FromJson(File.ReadAllText(settings.Model));
        }

        foreach (string file in settings.Files)
        {
            string outFile = Path.ChangeExtension(file, "html");

            string htmlString = BadHtmlTemplate.Create(file)
                                               .Run(model, opts);

            int originalSize = htmlString.Length;

            if (settings.Minify)
            {
                htmlString = htmlString.Replace("\n", " ")
                                       .Replace("\r", " ")
                                       .Replace("\t", " ");

                while (htmlString.Contains("  "))
                {
                    htmlString = htmlString.Replace("  ", " ");
                }

                BadConsole.WriteLine(string.Format("Minified output to {1} characters({0}%)",
                                                   Math.Round(htmlString.Length / (float)originalSize * 100, 2),
                                                   htmlString.Length
                                                  )
                                    );
            }
            else
            {
                BadConsole.WriteLine($"Generated output {htmlString.Length} characters");
            }

            BadFileSystem.WriteAllText(outFile, htmlString);
        }

        host?.Stop();

        return Task.FromResult(0);
    }
}