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
    /// <summary>
    /// Implements a Stack of File Systems that can be used to create a layered file system
    /// </summary>
    public class BadFileSystemStack
    {

        /// <summary>
        /// Creates a new BadFileSystemStack from a list of configs
        /// </summary>
        /// <param name="configs">Config Layers</param>
        /// <returns>A new BadFileSystemStack</returns>
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
        
        /// <summary>
        /// Config layer factories
        /// </summary>
        private readonly List<Func<BadLayeredFileSystemLayer>> m_FileSystemConfigs = new List<Func<BadLayeredFileSystemLayer>>();

        /// <summary>
        /// Adds a new layer to the stack from a directory
        /// </summary>
        /// <param name="config">Config Layer</param>
        /// <returns>The current BadFileSystemStack</returns>
        public BadFileSystemStack ConfigureLayer(Func<BadLayeredFileSystemLayer> config)
        {
            m_FileSystemConfigs.Add(config);
            return this;
        }

        /// <summary>
        /// Creates a Layered Filesystem from the stack
        /// </summary>
        /// <param name="createWritable">If true, the top layer of the stack will be a writable layer, if not a temporary layer is introduced at the top of the stack.</param>
        /// <returns>A new BadLayeredFileSystem</returns>
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