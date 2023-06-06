using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleCore.Systems;

namespace BadScript2.ConsoleCore;

public class BadConsoleRunner
{
	private readonly BadAConsoleSystem m_Default;
	private readonly BadAConsoleSystem[] m_Systems;

	public BadConsoleRunner(BadAConsoleSystem @default, params BadAConsoleSystem[] systems)
	{
		m_Default = @default;
		m_Systems = systems;
	}

	public int Run(string[] args)
	{
		while (args.Length == 0)
		{
			BadConsole.WriteLine("No command specified.");
			BadConsole.WriteLine("Usage: bs <system> [args]");
			BadConsole.WriteLine("Available systems:");

			foreach (BadAConsoleSystem sys in m_Systems)
			{
				BadConsole.WriteLine($"\t{sys.Name}");
			}

			BadConsole.Write("Input start arguments: ");

			args = BadConsole.ReadLine()!.Split(' ');
		}

		string name = args[0];
		BadAConsoleSystem? system = m_Systems.FirstOrDefault(x => x.Name == name);

		if (system == null)
		{
			return m_Default.Run(m_Default.Parse(args));
		}

		return system.Run(system.Parse(args.Skip(1).ToArray()));
	}
}
