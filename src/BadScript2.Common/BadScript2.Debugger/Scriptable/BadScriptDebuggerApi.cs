using System.IO;

using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Debugger.Scriptable;

/// <summary>
///     Implements the Debugger API for the Scriptable Debugger
/// </summary>
public class BadScriptDebuggerApi : BadInteropApi
{
	/// <summary>
	///     The Debugger Instance
	/// </summary>
	private readonly BadScriptDebugger m_Debugger;

	private readonly string m_DebuggerPath;

	/// <summary>
	///     Constructs a new BadScriptDebuggerApi instance
	/// </summary>
	/// <param name="debugger">The Debugger Instance</param>
	public BadScriptDebuggerApi(BadScriptDebugger debugger, string? path = null) : base("Debugger")
	{
		m_Debugger = debugger;
		m_DebuggerPath = path ?? BadScriptDebuggerSettings.Instance.DebuggerPath;
	}

	/// <summary>
	///     Registers a Function to the OnStep Event
	/// </summary>
	/// <param name="context">The Execution Context</param>
	/// <param name="func">The Function to be registered</param>
	public void RegisterStep(BadExecutionContext context, BadFunction func)
	{
		m_Debugger.OnStep += s =>
		{
			foreach (BadObject _ in func.Invoke(new[]
				         {
					         BadObject.Wrap(s),
				         },
				         context))
			{
				//Execute
			}
		};
	}

	/// <summary>
	///     Registers a Function to the OnFileLoaded Event
	/// </summary>
	/// <param name="context">The Execution Context</param>
	/// <param name="func">The Function to be registered</param>
	public void RegisterOnFileLoaded(BadExecutionContext context, BadFunction func)
	{
		m_Debugger.OnFileLoaded += s =>
		{
			foreach (BadObject _ in func.Invoke(new BadObject[]
				         {
					         s,
				         },
				         context))
			{
				//Execute
			}
		};
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetFunction<BadFunction>("RegisterStep", RegisterStep);
		target.SetFunction<BadFunction>("RegisterOnFileLoaded", RegisterOnFileLoaded);
		target.SetProperty("DebuggerPath", Path.GetFullPath(m_DebuggerPath));
	}
}
