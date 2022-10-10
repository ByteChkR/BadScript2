using System.Net.Sockets;
using System.Text;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine
{
    public class BadVirtualMachineManagerClientSystem : BadConsoleSystem<BadVirtualMachineManagerClientSystemSettings>
    {
        public override string Name => "vmc";

        protected override int Run(BadVirtualMachineManagerClientSystemSettings settings)
        {
            TcpClient client = new TcpClient(settings.Host, settings.Port);

            List<byte> data = new List<byte>();
            byte[] userData = Encoding.UTF8.GetBytes(settings.User);
            byte[] passwordData = Encoding.UTF8.GetBytes(settings.Password ?? "");
            byte[] vmNameData = Encoding.UTF8.GetBytes(settings.VmName);
            data.AddRange(BitConverter.GetBytes(userData.Length));
            data.AddRange(userData);
            data.AddRange(BitConverter.GetBytes(passwordData.Length));
            data.AddRange(passwordData);
            data.AddRange(BitConverter.GetBytes(vmNameData.Length));
            data.AddRange(vmNameData);
            client.GetStream().Write(data.ToArray());

            BadNetworkConsoleClient cc = new BadNetworkConsoleClient(client);
            cc.Start();

            return 0;
        }
    }
}