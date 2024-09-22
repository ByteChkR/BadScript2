using System;
using System.Collections.Generic;
using System.IO;

namespace BadScript2.Container
{
    public class BadFileSystemStackConfig
    {
        public string Type { get; set; }
        public string Source { get; set; }
        public string? Target { get; set; }

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