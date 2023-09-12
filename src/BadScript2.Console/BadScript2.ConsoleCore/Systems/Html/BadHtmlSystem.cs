using System.Net;
using System.Net.Sockets;

using BadHtml;

using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleAbstraction.Implementations.Remote;
using BadScript2.Debugger.Scriptable;
using BadScript2.Debugging;
using BadScript2.Interop.Json;
using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;
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
					args[0]
				}
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

		BadHtmlTemplateOptions opts = new BadHtmlTemplateOptions
		{
			SkipEmptyTextNodes = settings.SkipEmptyTextNodes
		};

		BadObject model = new BadTable();
		if (!string.IsNullOrEmpty(settings.Model))
		{
			model = BadJson.FromJson(File.ReadAllText(settings.Model));
		}

		foreach (string file in settings.Files)
		{
			string outFile = Path.ChangeExtension(file, "html");
			string htmlString = BadHtmlTemplate.Create(file).Run(model, opts);

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

				Console.WriteLine("Minified output to {1} characters({0}%)",
					Math.Round(htmlString.Length / (float)originalSize * 100, 2),
					htmlString.Length);
			}
			else
			{
				Console.WriteLine("Generated output {0} characters", htmlString.Length);
			}

			BadFileSystem.WriteAllText(outFile, htmlString);
		}

		host?.Stop();

		return -1;
	}
}
