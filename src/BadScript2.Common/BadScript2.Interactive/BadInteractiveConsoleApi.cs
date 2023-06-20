using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interactive;

/// <summary>
/// Implements the Interactive Console API
/// </summary>
public class BadInteractiveConsoleApi : BadInteropApi
{
	/// <summary>
	/// The Console Instance
	/// </summary>
	private readonly BadInteractiveConsole m_Console;

	/// <summary>
	/// Constructs a new BadInteractiveConsoleApi instance
	/// </summary>
	/// <param name="console">The Console Instance</param>
	public BadInteractiveConsoleApi(BadInteractiveConsole console) : base("Interactive")
	{
		m_Console = console;
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetFunction("Reset", m_Console.Reset);
		target.SetFunction<string>("Run", m_Console.Run);
		target.SetFunction<string>("Load", m_Console.Load);
		target.SetFunction<string>("RunIsolated", m_Console.RunIsolated);
		target.SetFunction<string>("LoadIsolated", m_Console.LoadIsolated);
		target.SetFunction("GetScope", GetScope);
		target.SetFunction<bool>("SetCatchError", SetCatchError);
		target.SetFunction<bool>("SetPreParse", SetPreParse);
	}

	/// <summary>
	/// Sets the Catch Error Flag
	/// </summary>
	/// <param name="enable">State</param>
	private void SetCatchError(bool enable)
	{
		m_Console.CatchErrors = enable;
	}

	/// <summary>
	/// Sets the Pre Parse Flag
	/// </summary>
	/// <param name="enable">State</param>
	private void SetPreParse(bool enable)
	{
		m_Console.PreParse = enable;
	}

	/// <summary>
	/// Returns the Current Scope
	/// </summary>
	/// <returns>Scope</returns>
	private BadObject GetScope()
	{
		return m_Console.CurrentScope ?? BadObject.Null;
	}
}
