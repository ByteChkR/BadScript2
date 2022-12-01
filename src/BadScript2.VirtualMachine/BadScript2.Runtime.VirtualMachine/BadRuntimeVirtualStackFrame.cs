using BadScript2.Runtime;

namespace BadScript2.VirtualMachine;

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