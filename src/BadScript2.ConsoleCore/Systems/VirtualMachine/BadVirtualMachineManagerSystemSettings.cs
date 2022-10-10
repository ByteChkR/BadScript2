using CommandLine;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine
{
    public class BadVirtualMachineManagerSystemSettings
    {
        [Value(0, HelpText = "The Virtual Machine Manager Path", Required = false)]
        public string MachineManagerPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "vm-service");
        [Option('p', "port", Required = true, HelpText = "Specifies the Host port.")]
        public int ManagerPort { get; set; } = 1337;
    }
}