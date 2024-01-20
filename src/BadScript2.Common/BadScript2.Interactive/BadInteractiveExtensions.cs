using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;

namespace BadScript2.Interactive;

public static class BadInteractiveExtensions
{
    public static void RunInteractive(this BadRuntime runtime, IEnumerable<string> files)
    {
        BadInteractiveConsole console = new BadInteractiveConsole(runtime, BadTaskRunner.Instance, files);

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
}