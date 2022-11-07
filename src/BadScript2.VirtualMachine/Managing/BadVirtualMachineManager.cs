using System;
using System.IO;
using System.Linq;

using BadScript2.IO;

using Newtonsoft.Json;

namespace BadScript2.VirtualMachine.Managing;

public class BadVirtualMachineException : Exception
{
    public BadVirtualMachineException( string message ) : base( message )
    {
    }
}
public class BadVirtualMachineManager
{
    private readonly IFileSystem m_FileSystem;
    private readonly string[] m_VirtualMachinePaths;

    public BadVirtualMachineManager(IFileSystem fileSystem, params string[] virtualMachinePaths)
    {
        m_FileSystem = fileSystem;
        m_VirtualMachinePaths = virtualMachinePaths;
    }

    private string? VirtualMachineSavePath => m_VirtualMachinePaths.FirstOrDefault();

    public BadVirtualMachineInfo? GetMachineInfo(string name)
    {
        foreach (string path in m_VirtualMachinePaths)
        {
            string fullPath = Path.Combine(path, name + ".vm.json");

            if (m_FileSystem.IsFile(fullPath))
            {
                return JsonConvert.DeserializeObject<BadVirtualMachineInfo>(m_FileSystem.ReadAllText(fullPath));
            }
        }


        return null;
    }

    public void SetMachineInfo(BadVirtualMachineInfo info)
    {
        string? vmPath = VirtualMachineSavePath;

        if (vmPath == null)
        {
            throw new BadVirtualMachineException("Can not save the virtual machine info");
        }

        string fullPath = Path.Combine(vmPath, info.Name + ".vm.json");

        m_FileSystem.WriteAllText(fullPath, JsonConvert.SerializeObject(info));
    }

    public void DeleteMachineInfo(string name)
    {
        foreach (string path in m_VirtualMachinePaths)
        {
            string fullPath = Path.Combine(path, name);

            if (m_FileSystem.IsFile(fullPath))
            {
                m_FileSystem.DeleteFile(fullPath);
            }
        }
    }
}