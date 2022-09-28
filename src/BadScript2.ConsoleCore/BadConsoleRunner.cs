using BadScript2.ConsoleCore.Systems;

namespace BadScript2.ConsoleCore
{
    public class BadConsoleRunner
    {
        private readonly BadConsoleSystem m_Default;
        private readonly BadConsoleSystem[] m_Systems;

        public BadConsoleRunner(BadConsoleSystem @default, params BadConsoleSystem[] systems)
        {
            m_Default = @default;
            m_Systems = systems;
        }

        public int Run(string[] args)
        {
            while (args.Length == 0)
            {
                Console.WriteLine("No command specified.");
                Console.WriteLine("Usage: bs <system> [args]");
                Console.WriteLine("Available systems:");
                foreach (BadConsoleSystem sys in m_Systems)
                {
                    Console.WriteLine($"\t{sys.Name}");
                }

                Console.Write("Input start arguments: ");

                args = Console.ReadLine()!.Split(' ');
            }

            string name = args[0];
            BadConsoleSystem? system = m_Systems.FirstOrDefault(x => x.Name == name);

            if (system == null)
            {
                return m_Default.Run(m_Default.Parse(args));
            }

            return system.Run(system.Parse(args.Skip(1).ToArray()));
        }
    }
}