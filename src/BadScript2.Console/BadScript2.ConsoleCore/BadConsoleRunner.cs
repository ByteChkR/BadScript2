using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleCore.Systems;

namespace BadScript2.ConsoleCore;

/// <summary>
///     Class that can register console systems and run them
/// </summary>
public class BadConsoleRunner
{
	/// <summary>
	///     The Default system that is beeing used if the user does not specify a system
	/// </summary>
	private readonly BadAConsoleSystem m_Default;

	/// <summary>
	///     The Systems that are registered
	/// </summary>
	private readonly BadAConsoleSystem[] m_Systems;

	/// <summary>
	///     Constructor
	/// </summary>
	/// <param name="default">Default System</param>
	/// <param name="systems">Registered Systems</param>
	public BadConsoleRunner(BadAConsoleSystem @default, params BadAConsoleSystem[] systems)
    {
        m_Default = @default;
        m_Systems = systems;
    }

	/// <summary>
	///     Runs a system with the specified arguments
	/// </summary>
	/// <param name="args">Commandline Arguments</param>
	/// <returns>Return Code</returns>
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

            args = BadConsole.ReadLine().Split(' ');
        }

        string name = args[0];
        BadAConsoleSystem? system = m_Systems.FirstOrDefault(x => x.Name == name);

        return system?.Run(system.Parse(args.Skip(1).ToArray())) ?? m_Default.Run(m_Default.Parse(args));
    }
}