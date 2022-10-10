using BadScript2.VirtualMachine;

using Newtonsoft.Json;

namespace BadScript2.ConsoleCore.Systems.VirtualMachine
{
    public class BadVirtualMachineNewSystem : BadConsoleSystem<BadVirtualMachineNewSystemSettings>
    {
        public override string Name => "vm-new";

        protected override int Run(BadVirtualMachineNewSystemSettings settings)
        {
            BadVirtualMachineInfo info = new BadVirtualMachineInfo();
            info.Name = settings.Name;
            info.Mounts = settings.FileSystemMounts.Select(x => JsonConvert.DeserializeObject<BadFileSystemMount>(File.ReadAllText(x))).ToArray()!;
            string outputPath;
            if (settings.OutputPath == null)
            {
                outputPath = "./" + info.Name + ".vm.json";
            }
            else
            {
                outputPath = settings.OutputPath;
            }

            File.WriteAllText(outputPath, JsonConvert.SerializeObject(info, Formatting.Indented));

            return 0;
        }
    }
}