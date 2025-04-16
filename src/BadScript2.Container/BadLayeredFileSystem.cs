using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript2.IO;
using BadScript2.IO.Virtual;

/// <summary>
/// Contains a layered file system/containerization implementation.
/// </summary>
namespace BadScript2.Container
{

    /// <summary>
    /// Implements a layered file system.
    /// </summary>
    public class BadLayeredFileSystem : IVirtualFileSystem
    {
        /// <summary>
        /// The layers of the file system.
        /// </summary>
        private readonly BadLayeredFileSystemLayer[] m_Layers;
        /// <summary>
        /// All layers of the file system.
        /// </summary>
        public IReadOnlyList<IBadLayeredFileSystemLayer> Layers => m_Layers;

        /// <summary>
        /// Returns information about the writable layers states.
        /// </summary>
        /// <returns>Information about the writable layers states.</returns>
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
        /// <summary>
        /// Creates a new layered file system.
        /// </summary>
        /// <param name="layers">The layers of the file system.</param>
        public BadLayeredFileSystem(params BadLayeredFileSystemLayer[] layers)
        {
            m_Layers = layers;
        }

        /// <summary>
        /// Returns true if the content of the two streams is equal.
        /// </summary>
        /// <param name="s1">stream 1</param>
        /// <param name="s2">stream 2</param>
        /// <returns>true if the content of the two streams is equal.</returns>
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
        
        /// <summary>
        /// Optimizes the file system by removing files that are present in the writable layer and have the same content as files in the read-only layers.
        /// </summary>
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
        
        /// <summary>
        /// Removes a file or directory from the writable layer.
        /// </summary>
        /// <param name="path">The path to the file or directory to remove.</param>
        /// <returns>true if the file or directory was removed, false otherwise.</returns>
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
        /// <inheritdoc />
        public string GetStartupDirectory()
        {
            return GetWritable().GetStartupDirectory();
        }

        /// <inheritdoc />
        public bool Exists(string path)
        {
            return m_Layers.Any(x => x.FileSystem.Exists(path));
        }

        /// <inheritdoc />
        public bool IsFile(string path)
        {
            return m_Layers.Any(x => x.FileSystem.IsFile(path));
        }

        /// <inheritdoc />
        public bool IsDirectory(string path)
        {
            return m_Layers.Any(x => x.FileSystem.IsDirectory(path));
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
        {
            return m_Layers.SelectMany(x => 
                x.FileSystem.Exists(path) && 
                x.FileSystem.IsDirectory(path) ? 
                    x.FileSystem.GetFiles(path, extension, recursive) : 
                    Enumerable.Empty<string>()).Distinct();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetDirectories(string path, bool recursive)
        {
            return m_Layers.SelectMany(x => 
                x.FileSystem.Exists(path) && 
                x.FileSystem.IsDirectory(path) ? 
                    x.FileSystem.GetDirectories(path, recursive) : 
                    Enumerable.Empty<string>()).Distinct();
        }

        /// <inheritdoc />
        public void CreateDirectory(string path, bool recursive = false)
        {
            GetWritable().CreateDirectory(path, recursive);
        }

        /// <inheritdoc />
        public void DeleteDirectory(string path, bool recursive)
        {
            if (!GetWritable().IsDirectory(path)) throw new Exception("Is Readonly");
            GetWritable().DeleteDirectory(path, recursive);
        }

        /// <inheritdoc />
        public void DeleteFile(string path)
        {
            if (!GetWritable().IsFile(path)) throw new Exception("Is Readonly");
            GetWritable().DeleteFile(path);
        }

        /// <inheritdoc />
        public string GetFullPath(string path)
        {
            return GetWritable().GetFullPath(path);
        }

        /// <inheritdoc />
        public Stream OpenRead(string path)
        {
            var fs = m_Layers.Last(x => x.FileSystem.IsFile(path))?.FileSystem ?? GetWritable();
            return fs.OpenRead(path);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public string GetCurrentDirectory()
        {
            return GetWritable().GetCurrentDirectory();
        }

        /// <inheritdoc />
        public void SetCurrentDirectory(string path)
        {
            foreach (var fs in m_Layers) fs.FileSystem.SetCurrentDirectory(path);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Move(string src, string dst, bool overwrite = true)
        {
            Copy(src, dst, overwrite);

            if (IsDirectory(src))
                DeleteDirectory(src, true);
            else
                DeleteFile(src);
        }

        /// <summary>
        /// Returns the writable layer of the file system.
        /// </summary>
        /// <returns>The writable layer of the file system.</returns>
        public BadVirtualFileSystem GetWritable()
        {
            return m_Layers.Last().FileSystem;
        }

        /// <summary>
        /// Returns true if the sub path is a subfolder of the root path.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="sub">The sub path.</param>
        /// <returns></returns>
        private bool IsSubfolderOf(string root, string sub)
        {
            return GetFullPath(sub).StartsWith(GetFullPath(root));
        }

        /// <summary>
        /// Copies a directory to another directory.
        /// </summary>
        /// <param name="src">Source directory</param>
        /// <param name="dst">Destination directory</param>
        private void CopyDirectoryToDirectory(string src, string dst)
        {
            foreach (var directory in GetDirectories(src, true)) CreateDirectory(directory);

            foreach (var file in GetFiles(src, "*", true)) CopyFileToFile(file, file.Replace(src, dst));
        }

        /// <summary>
        /// Copies a file to another file.
        /// </summary>
        /// <param name="src">Source file</param>
        /// <param name="dst">Destination file</param>
        private void CopyFileToFile(string src, string dst)
        {
            using var s = OpenRead(src);
            using var d = OpenWrite(dst, BadWriteMode.CreateNew);
            s.CopyTo(d);
        }
    }
}