using BadScript2.Console.Systems;

namespace BadScript2.Console;

internal class BadConsoleRunner
{
    private readonly BadConsoleSystem[] m_Systems;

    public BadConsoleRunner(params BadConsoleSystem[] systems)
    {
        m_Systems = systems;
    }

    public int Run(string[] args)
    {
        while (args.Length == 0)
        {
            System.Console.WriteLine("No command specified.");
            System.Console.WriteLine("Usage: bs <system> [args]");
            System.Console.WriteLine("Available systems:");
            foreach (BadConsoleSystem sys in m_Systems)
            {
                System.Console.WriteLine($"\t{sys.Name}");
            }

            System.Console.Write("Input start arguments: ");

            args = System.Console.ReadLine()!.Split(' ');
        }

        string name = args[0];
        BadConsoleSystem? system = m_Systems.FirstOrDefault(x => x.Name == name);

        if (system == null)
        {
            System.Console.WriteLine("Unknown command");

            return -1;
        }

        return system.Run(system.Parse(args.Skip(1).ToArray()));
    }
}