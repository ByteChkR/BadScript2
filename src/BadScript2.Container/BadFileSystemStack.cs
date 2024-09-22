using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Runtime.Settings;

namespace BadScript2.Container
{
    public class BadFileSystemStack
    {

        public static async Task<BadFileSystemStack> FromConfigs(params BadFileSystemStackConfig[] configs)
        {
            var stack = new BadFileSystemStack();
            foreach (var config in configs)
            {
                if (config.Type == "Directory")
                {
                    stack.FromDirectory(config.Source, config.Target ?? "/");
                }
                else if (config.Type == "Zip")
                {
                    stack.FromZip(config.Source, config.Target ?? "/");
                }
                else if (config.Type == "Web")
                {
                    await stack.FromWebArchive(config.Source, config.Target ?? "/");
                }
            }

            return stack;
        }
        
        private readonly List<Func<BadVirtualFileSystem>> m_FileSystemConfigs = new List<Func<BadVirtualFileSystem>>();

        public BadFileSystemStack ConfigureLayer(Func<BadVirtualFileSystem> config)
        {
            m_FileSystemConfigs.Add(config);
            return this;
        }

        public IFileSystem Create()
        {
            var fileSystems = new List<BadVirtualFileSystem>();
            foreach (var config in m_FileSystemConfigs) fileSystems.Add(config());

            fileSystems.Add(new BadVirtualFileSystem());
            
            var fs= new BadLayeredFileSystem(fileSystems.ToArray());

            fs.SetCurrentDirectory("/");
            return fs;
        }
    }
}