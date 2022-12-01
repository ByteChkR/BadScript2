using System;

namespace BadScript2.VirtualMachine.Managing;

public class BadVirtualMachineException : Exception
{
    public BadVirtualMachineException(string message) : base(message) { }
}