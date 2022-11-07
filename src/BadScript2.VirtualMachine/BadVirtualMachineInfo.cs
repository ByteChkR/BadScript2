using System;

namespace BadScript2.VirtualMachine;

public class BadVirtualMachineInfo
{
    public BadFileSystemMount[] Mounts { get; set; } = Array.Empty<BadFileSystemMount>();
    public string Name { get; set; } = "VirtualMachine";
}