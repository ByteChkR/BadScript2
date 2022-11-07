using CommandLine;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine;

public class BadVirtualMachineManagerClientSystemSettings
{
    [Option('h', "host", Required = false, HelpText = "Specifies the host name.")]
    public string Host { get; set; } = "localhost";

    [Option("port", Required = false, HelpText = "Specifies the port.")]
    public int Port { get; set; } = 1234;

    [Option('u', "user", Required = false, HelpText = "Specifies the user name.")]
    public string User { get; set; } = "anon";

    [Option('p', "password", Required = false, HelpText = "Specifies the user password.")]
    public string Password { get; set; } = "";

    [Option('n', "name", Required = true, HelpText = "Specifies the vm name.")]
    public string VmName { get; set; } = "BadOS";
}