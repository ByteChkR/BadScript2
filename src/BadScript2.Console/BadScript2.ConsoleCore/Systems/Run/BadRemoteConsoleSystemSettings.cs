using CommandLine;

namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
///     Remote Console System Settings
/// </summary>
public class BadRemoteConsoleSystemSettings
{
	/// <summary>
	///     The Host to connect to
	/// </summary>
	[Value(0, Default = "localhost", HelpText = "The Host to connect to", Required = true)]
	public string Host { get; set; } = "localhost";

	/// <summary>
	///     The Host port to connect to
	/// </summary>
	[Value(1, Default = 1337, HelpText = "The Host port to connect to", Required = true)]
	public int Port { get; set; } = 1337;
}
