using System;
using System.Threading.Tasks;

namespace BadScript2.ConsoleAbstraction;

/// <summary>
///     Interface that abstracts the console
/// </summary>
public interface IBadConsole
{
	/// <summary>
	///     The Foreground Color
	/// </summary>
	ConsoleColor ForegroundColor { get; set; }

	/// <summary>
	///     The Background Color
	/// </summary>
	ConsoleColor BackgroundColor { get; set; }

	/// <summary>
	///     Writes a string to the console
	/// </summary>
	/// <param name="str">The string to be written</param>
	void Write(string str);

	/// <summary>
	///     Writes a string to the console and appends a newline
	/// </summary>
	/// <param name="str">The string to be written</param>
	void WriteLine(string str);

	/// <summary>
	///     Reads a line from the console
	/// </summary>
	/// <returns>The line that was read</returns>
	string ReadLine();

	/// <summary>
	///     Reads a line from the console asynchronously
	/// </summary>
	/// <returns>The line that was read</returns>
	Task<string> ReadLineAsync();

	/// <summary>
	///     Clears the console
	/// </summary>
	void Clear();
}
