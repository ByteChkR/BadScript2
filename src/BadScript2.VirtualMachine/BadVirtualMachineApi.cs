using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.VirtualMachine
{
    public class BadVirtualMachineApi : BadInteropApi
    {
        private readonly BadVirtualMachine m_Machine;

        public BadVirtualMachineApi(BadVirtualMachine machine) : base("Machine")
        {
            m_Machine = machine;
        }

        public override void Load(BadTable target)
        {
            target.SetFunction("GetName", _ => m_Machine.Info.Name);
            target.SetFunction("Exit", _ => m_Machine.ForceExit());
            target.SetFunction("Reboot", _ => m_Machine.Reboot());
        }
    }
}