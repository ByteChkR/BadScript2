using System.Net.Sockets;

using BadScript2.IO;
using BadScript2.VirtualMachine.Managing;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine
{
    public class BadVirtualMachineManagerSystem : BadConsoleSystem<BadVirtualMachineManagerSystemSettings>
    {
        public override string Name => "vms";

        protected override int Run(BadVirtualMachineManagerSystemSettings settings)
        {
            using BadVirtualMachineService vmService = new BadVirtualMachineService(settings.MachineManagerPath, BadFileSystem.Instance);

            BadVirtualMachineManagerHost host = new BadVirtualMachineManagerHost(vmService, TcpListener.Create(settings.ManagerPort));
            host.StartSynchronously();

            return -1;
        }
    }
}