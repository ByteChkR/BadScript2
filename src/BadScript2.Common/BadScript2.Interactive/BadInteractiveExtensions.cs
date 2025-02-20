using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;

namespace BadScript2.Interactive;

/// <summary>
///     Interactive Console Extensions for the BadScript Runtime
/// </summary>
public static class BadInteractiveExtensions
{
    /// <summary>
    ///     Runs the Interactive Console
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    /// <param name="files">The Files to load before the Interactive Session begins</param>
    public static void RunInteractive(this BadRuntime runtime, IEnumerable<string> files)
    {
        using BadInteractiveConsole console = new BadInteractiveConsole(runtime, BadTaskRunner.Instance, files);

        while (true)
        {
            BadConsole.Write(">");
            string cmd = BadConsole.ReadLine();

            if (cmd == "exit")
            {
                return;
            }

            console.Run(cmd);
        }
    }

    public static async Task RunInteractiveAsync(this BadRuntime runtime, IEnumerable<string> files)
    {
        using BadInteractiveConsole console = new BadInteractiveConsole(runtime, BadTaskRunner.Instance, files);

        while (true)
        {
            BadConsole.Write(">");
            string cmd = await BadConsole.ReadLineAsync();

            if (cmd == "exit")
            {
                return;
            }

            console.Run(cmd);
        }
    }
}