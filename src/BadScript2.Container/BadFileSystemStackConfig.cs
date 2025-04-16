using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BadScript2.Container
{
    /// <summary>
    /// A config for a file system stack.
    /// </summary>
    public class BadFileSystemStackConfig
    {
        /// <summary>
        /// The name of the config.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// The Type of layer to create.
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// The source of the layer. This can be a directory, zip file or web archive.
        /// </summary>
        public string Source { get; set; } = string.Empty;
        /// <summary>
        /// The target of the layer. This is the path where the layer will be mounted.
        /// </summary>
        public string? Target { get; set; }

        /// <summary>
        /// Creates a new BadFileSystemStackConfig from a Newline separated file.
        /// Example:
        ///     Zip|path/to/root.zip
        ///     Directory|path/to/home|/user/home/tim
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>An array of BadFileSystemStackConfig.</returns>
        public static BadFileSystemStackConfig[] FromFile(string path)
        {
            List<BadFileSystemStackConfig> configs = new List<BadFileSystemStackConfig>();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    continue;
                }
                configs.Add(Parse(line));
            }

            return configs.ToArray();
        }
        
        /// <summary>
        /// Parses a line from the config file.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>A BadFileSystemStackConfig.</returns>
        /// <exception cref="Exception">Gets raised if the line is not in the correct format.</exception>
        public static BadFileSystemStackConfig Parse(string line)
        {
            var parts = line.Split('|');
            if (parts.Length != 2 && parts.Length != 3)
            {
                throw new Exception("Invalid Format");
            }
            var type = parts[0];
            var source = parts[1];
            var target = "/";
            if (parts.Length == 3)
            {
                target = parts[2];
            }

            return new BadFileSystemStackConfig
            {
                Type = type,
                Source = source,
                Target = target
            };
        }
    }
}