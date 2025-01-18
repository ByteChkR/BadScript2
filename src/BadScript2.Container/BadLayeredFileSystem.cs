using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Utility;

namespace BadScript2.Container
{

    public class BadLayeredFileSystem : IVirtualFileSystem
    {
        private readonly BadLayeredFileSystemLayer[] m_Layers;
        public IReadOnlyList<IBadLayeredFileSystemLayer> Layers => m_Layers;

        public BadLayeredFileSystemStackInfo GetInfo()
        {
            List<BadLayeredFileSystemInfo> fileSystems = new List<BadLayeredFileSystemInfo>();
            Dictionary<string,BadLayeredFileSystemFileInfo> files = new Dictionary<string,BadLayeredFileSystemFileInfo>();
            foreach (var layer in m_Layers)
            {
                var fs = layer.FileSystem;
                fileSystems.Add(new BadLayeredFileSystemInfo() { Writable = fs == GetWritable(), Name = layer.Name, MetaData = layer.MetaData });
                foreach (var file in fs.GetFiles("/", "", true))
                {
                    if (!files.TryGetValue(file, out var info))
                    {
                        info = new BadLayeredFileSystemFileInfo()
                        {
                            Path = file,
                            PresentIn = new List<string>()
                        };
                        files[file] = info;
                    }
                    
                    info.PresentIn.Add(layer.Name);
                }
            }
            
            return new BadLayeredFileSystemStackInfo() { FileSystems = fileSystems.ToArray(), Files = files.Values.ToArray() };
        }
        public BadLayeredFileSystem(params BadLayeredFileSystemLayer[] layers)
        {
            m_Layers = layers;
        }

        private bool ContentEquals(Stream s1, Stream s2)
        {
            if (s1.Length != s2.Length) return false;
            int b1, b2;
            do
            {
                b1 = s1.ReadByte();
                b2 = s2.ReadByte();
                if (b1 != b2) return false;
            } while (b1 != -1);
            return true;
        }
        public void Optimize()
        {
            //Ensure that the writable layer does not have a file that already exists in a read-only layer and has the same content
            var writable = GetWritable();
            foreach (var file in writable.GetFiles("/", "", true).ToArray())
            {
                foreach (var layer in m_Layers.SkipLast(1))
                {
                    if (layer.FileSystem.Exists(file) && layer.FileSystem.IsFile(file))
                    {
                        bool equals;
                        using (var wStream = writable.OpenRead(file))
                        {
                            using var rStream = layer.FileSystem.OpenRead(file);
                            equals = ContentEquals(wStream, rStream);
                        }
                        if (equals)
                        {
                            writable.DeleteFile(file);
                            break;
                        }
                    }
                }
            }
        }
        public bool Restore(string path)
        {
            var writable = GetWritable();
            if (writable.Exists(path))
            {
                if (writable.IsFile(path))
                {
                    writable.DeleteFile(path);
                }
                else
                {
                    writable.DeleteDirectory(path, true);
                }
                return true;
            }
            return false;
        }
        public string GetStartupDirectory()
        {
            return GetWritable().GetStartupDirectory();
        }

        public bool Exists(string path)
        {
            return m_Layers.Any(x => x.FileSystem.Exists(path));
        }

        public bool IsFile(string path)
        {
            return m_Layers.Any(x => x.FileSystem.IsFile(path));
        }

        public bool IsDirectory(string path)
        {
            return m_Layers.Any(x => x.FileSystem.IsDirectory(path));
        }

        public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
        {
            return m_Layers.SelectMany(x => 
                x.FileSystem.Exists(path) && 
                x.FileSystem.IsDirectory(path) ? 
                    x.FileSystem.GetFiles(path, extension, recursive) : 
                    Enumerable.Empty<string>()).Distinct();
        }

        public IEnumerable<string> GetDirectories(string path, bool recursive)
        {
            return m_Layers.SelectMany(x => 
                x.FileSystem.Exists(path) && 
                x.FileSystem.IsDirectory(path) ? 
                    x.FileSystem.GetDirectories(path, recursive) : 
                    Enumerable.Empty<string>()).Distinct();
        }

        public void CreateDirectory(string path, bool recursive = false)
        {
            GetWritable().CreateDirectory(path, recursive);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            if (!GetWritable().IsDirectory(path)) throw new Exception("Is Readonly");
            GetWritable().DeleteDirectory(path, recursive);
        }

        public void DeleteFile(string path)
        {
            if (!GetWritable().IsFile(path)) throw new Exception("Is Readonly");
            GetWritable().DeleteFile(path);
        }

        public string GetFullPath(string path)
        {
            return GetWritable().GetFullPath(path);
        }

        public Stream OpenRead(string path)
        {
            var fs = m_Layers.Last(x => x.FileSystem.IsFile(path))?.FileSystem ?? GetWritable();
            return fs.OpenRead(path);
        }

        public Stream OpenWrite(string path, BadWriteMode mode)
        {
            var writable = GetWritable();
            var dir = Path.GetDirectoryName(path);
            if (dir != null && IsDirectory(dir) && !writable.IsDirectory(dir))
            {
                //Create the directory if it does not exist(it exists in some other layer, we need to create it in the writable layer)
                writable.CreateDirectory(dir, true);
            }
            //In order to properly work with Append mode, we need to copy the file to the writable file system if it does not exist
            if (mode == BadWriteMode.Append && !writable.IsFile(path))
            {
                using var src = OpenRead(path);
                using var dst = writable.OpenWrite(path, BadWriteMode.CreateNew);
                src.CopyTo(dst);
            }
            return writable.OpenWrite(path, mode);
        }

        public string GetCurrentDirectory()
        {
            return GetWritable().GetCurrentDirectory();
        }

        public void SetCurrentDirectory(string path)
        {
            foreach (var fs in m_Layers) fs.FileSystem.SetCurrentDirectory(path);
        }

        public void Copy(string src, string dst, bool overwrite = true)
        {
            if (IsDirectory(src))
            {
                if (IsSubfolderOf(src, dst)) throw new IOException("Cannot copy a directory to a subfolder of itself.");

                if (!overwrite && IsDirectory(src)) throw new IOException("Directory already exists.");

                CopyDirectoryToDirectory(src, dst);
            }
            else if (IsFile(src))
            {
                if (!overwrite && IsFile(src)) throw new IOException("File already exists.");

                CopyFileToFile(src, dst);
            }
            else
            {
                throw new IOException("Source path is not a file or directory");
            }
        }

        public void Move(string src, string dst, bool overwrite = true)
        {
            Copy(src, dst, overwrite);

            if (IsDirectory(src))
                DeleteDirectory(src, true);
            else
                DeleteFile(src);
        }

        public BadVirtualFileSystem GetWritable()
        {
            return m_Layers.Last().FileSystem;
        }

        private bool IsSubfolderOf(string root, string sub)
        {
            return GetFullPath(sub).StartsWith(GetFullPath(root));
        }

        private void CopyDirectoryToDirectory(string src, string dst)
        {
            foreach (var directory in GetDirectories(src, true)) CreateDirectory(directory);

            foreach (var file in GetFiles(src, "*", true)) CopyFileToFile(file, file.Replace(src, dst));
        }

        private void CopyFileToFile(string src, string dst)
        {
            using var s = OpenRead(src);
            using var d = OpenWrite(dst, BadWriteMode.CreateNew);
            s.CopyTo(d);
        }
    }
}