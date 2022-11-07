using System;

namespace BadScript2.VirtualMachine;

public class BadVirtualMachineInfo
{
    public BadFileSystemMount[] Mounts = Array.Empty<BadFileSystemMount>();
    public string Name = "VirtualMachine";
}