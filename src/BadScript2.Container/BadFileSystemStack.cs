using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Runtime.Settings;
using Newtonsoft.Json.Linq;

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
                    stack.FromDirectory(config.Source, config.Target ?? "/", config.Name);
                }
                else if (config.Type == "Zip")
                {
                    stack.FromZip(config.Source, config.Target ?? "/", config.Name);
                }
                else if (config.Type == "Web")
                {
                    await stack.FromWebArchive(config.Source, config.Target ?? "/", config.Name);
                }
            }

            return stack;
        }
        
        private readonly List<Func<BadLayeredFileSystemLayer>> m_FileSystemConfigs = new List<Func<BadLayeredFileSystemLayer>>();

        public BadFileSystemStack ConfigureLayer(Func<BadLayeredFileSystemLayer> config)
        {
            m_FileSystemConfigs.Add(config);
            return this;
        }

        public BadLayeredFileSystem Create(bool createWritable = false)
        {
            var fileSystems = new List<BadLayeredFileSystemLayer>();
            foreach (var config in m_FileSystemConfigs) fileSystems.Add(config());

            if(!createWritable)
                fileSystems.Add(new BadLayeredFileSystemLayer
                {
                    Name = "Writable Layer",
                    FileSystem = new BadVirtualFileSystem(),
                    MetaData = new JObject
                    {
                        ["AutoCreated"] = true
                    }
                });
            
            var fs= new BadLayeredFileSystem(fileSystems.ToArray());

            fs.SetCurrentDirectory("/");
            return fs;
        }
    }
}