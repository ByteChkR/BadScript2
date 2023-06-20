using System;
using System.Threading.Tasks;

namespace BadScript2.ConsoleAbstraction.Implementations;

/// <summary>
/// Implements a wrapper for the default system console
/// </summary>
public class SystemConsole : IBadConsole
{
	public void Write(string str)
	{
		Console.Write(str);
	}

	public void WriteLine(string str)
	{
		Console.WriteLine(str);
	}

	public string ReadLine()
	{
		return Console.ReadLine();
	}

	public Task<string> ReadLineAsync()
	{
		return Task.Run(Console.ReadLine);
	}

	public void Clear()
	{
		Console.Clear();
	}

	public ConsoleColor ForegroundColor
	{
		get => Console.ForegroundColor;
		set => Console.ForegroundColor = value;
	}

	public ConsoleColor BackgroundColor
	{
		get => Console.BackgroundColor;
		set => Console.BackgroundColor = value;
	}
}
