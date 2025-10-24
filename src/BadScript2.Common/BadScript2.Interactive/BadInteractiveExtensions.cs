using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;
using BadScript2.IO;

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
    /// <param name="fs">The File System to use</param>
    /// <param name="files">The Files to load before the Interactive Session begins</param>
    public static void RunInteractive(this BadRuntime runtime, IFileSystem fs, IEnumerable<string> files)
    {
        using BadInteractiveConsole console = new BadInteractiveConsole(runtime, fs, new BadTaskRunner(), files);

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

    /// <summary>
    /// Runs the Interactive Console Asynchronously
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    /// <param name="fs">The File System to use</param>
    /// <param name="files">The Files to load before the Interactive Session begins</param>
    public static async Task RunInteractiveAsync(this BadRuntime runtime, IFileSystem fs, IEnumerable<string> files)
    {
        using BadInteractiveConsole console = new BadInteractiveConsole(runtime, fs, new BadTaskRunner(), files);

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