using System;
/// <summary>
/// Contains the Implementation of the Remote Console Abstraction over TCP
/// </summary>
namespace BadScript2.ConsoleAbstraction.Implementations.Remote;

/// <summary>
///     Exception that is thrown when the remote console encounters an error
/// </summary>
public class BadNetworkConsoleException : Exception
{
	/// <summary>
	///     Creates a new Exception
	/// </summary>
	/// <param name="message">The Exception Message</param>
	public BadNetworkConsoleException(string message) : base(message) { }
}