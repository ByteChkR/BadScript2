using BadScript2.Common.Logging;
using BadScript2.Common.Logging.Writer;
using BadScript2.ConsoleAbstraction.Implementations;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

using Newtonsoft.Json;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine;

public class BadVirtualMachineRunSystem : BadConsoleSystem<BadVirtualMachineRunSystemSettings>
{
    public override string Name => "vm";

    protected override int Run(BadVirtualMachineRunSystemSettings settings)
    {
        BadLogWriterSettings.Instance.Mask = BadLogMask.None;
        BadVirtualMachineInfo info = JsonConvert.DeserializeObject<BadVirtualMachineInfo>(File.ReadAllText(settings.FilePath))!;

        BadVirtualMachine vm = new BadVirtualMachine(info);
        foreach (BadObject o in vm.Execute(new SystemConsole())) { }

        vm.SaveState();

        return 0;
    }
}