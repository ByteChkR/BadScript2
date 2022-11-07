using CommandLine;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine;

public class BadVirtualMachineRunSystemSettings
{
    [Value(0, HelpText = "The File Path of the Virtual Machine Config")]
    public string FilePath { get; set; } = "vm.json";
}