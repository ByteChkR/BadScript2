using CommandLine;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine
{
    public class BadVirtualMachineNewSystemSettings
    {
        [Option('n', "name", Required = true, HelpText = "The virtual machine name.")]
        public string Name { get; set; } = "New Virtual Machine";

        [Option('m', "mounts", Required = false, HelpText = "The virtual machine mount files.")]
        public IEnumerable<string> FileSystemMounts { get; set; } = new List<string>();

        [Option('o', "output", Required = false, HelpText = "The virtual machine output file.")]
        public string? OutputPath { get; set; }
    }
}