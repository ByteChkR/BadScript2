namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Stores the current execution state of the Virtual Machine.
/// </summary>
public class BadRuntimeVirtualStackFrame
{
	public readonly BadExecutionContext Context;
	public int BreakPointer = -1;
	public int ContinuePointer = -1;
	public int CreatePointer = 0;
	public int ReturnPointer = -1;
	public int ThrowPointer = -1;

	public BadRuntimeVirtualStackFrame(BadExecutionContext context)
	{
		Context = context;
	}
}
