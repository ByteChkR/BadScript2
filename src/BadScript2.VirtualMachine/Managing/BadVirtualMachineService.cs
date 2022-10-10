using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript2.IO;

using Newtonsoft.Json;

namespace BadScript2.VirtualMachine.Managing
{
    public class BadVirtualMachineService : IDisposable
    {
        private readonly IFileSystem m_FileSystem;
        private readonly string m_MachineServicePath;
        private readonly Dictionary<BadVirtualMachineUser, BadVirtualMachineManager> m_Managers = new Dictionary<BadVirtualMachineUser, BadVirtualMachineManager>();
        private readonly List<BadVirtualMachineUser> m_Users = new List<BadVirtualMachineUser>();

        public BadVirtualMachineService(string machineServicePath, IFileSystem fileSystem)
        {
            m_MachineServicePath = machineServicePath;
            m_FileSystem = fileSystem;
            m_FileSystem.CreateDirectory(Path.Combine(machineServicePath, "global"));
            m_FileSystem.CreateDirectory(Path.Combine(machineServicePath, "users"));
            LoadUsers();
        }

        public BadVirtualMachineUser? Authenticate(string name, string password)
        {
            return m_Users.FirstOrDefault(x => x.Name == name && x.Password == password);
        }

        public BadVirtualMachineManager GetManager(BadVirtualMachineUser user)
        {
            BadVirtualMachineManager manager;
            if (!m_Managers.ContainsKey(user))
            {
                manager = new BadVirtualMachineManager(
                    m_FileSystem,
                    Path.Combine(m_MachineServicePath, "users", user.Name),
                    Path.Combine(m_MachineServicePath, "global")
                );
                m_Managers[user] = manager;
            }
            else
            {
                manager = m_Managers[user];
            }

            return manager;
        }

        public void LoadUsers()
        {
            string usersFile = Path.Combine(m_MachineServicePath, "users.json");

            if (!m_FileSystem.IsFile(usersFile))
            {
                m_Users.Add(BadVirtualMachineUser.Anonymous);
                m_FileSystem.CreateDirectory(Path.Combine(m_MachineServicePath, "users", BadVirtualMachineUser.Anonymous.Name));
                SaveUsers();
                return;
            }

            string json = m_FileSystem.ReadAllText(usersFile);
            BadVirtualMachineUser[] users = JsonConvert.DeserializeObject<BadVirtualMachineUser[]>(json)!;
            foreach (BadVirtualMachineUser user in users)
            {
                m_FileSystem.CreateDirectory(Path.Combine(m_MachineServicePath, "users", user.Name));
            }

            m_Users.AddRange(users);
        }

        public void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(m_Users, Formatting.Indented);
            m_FileSystem.WriteAllText(Path.Combine(m_MachineServicePath, "users.json"), json);
        }

        public void Dispose()
        {
            SaveUsers();
        }
    }
}