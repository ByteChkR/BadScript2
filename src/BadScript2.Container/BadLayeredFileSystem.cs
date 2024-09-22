using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript2.IO;
using BadScript2.IO.Virtual;

namespace BadScript2.Container
{
    internal class BadLayeredFileSystem : IFileSystem
    {
        private readonly BadVirtualFileSystem[] m_FileSystems;

        public BadLayeredFileSystem(params BadVirtualFileSystem[] fileSystems)
        {
            m_FileSystems = fileSystems;
        }

        public string GetStartupDirectory()
        {
            return GetWritable().GetStartupDirectory();
        }

        public bool Exists(string path)
        {
            return m_FileSystems.Any(x => x.Exists(path));
        }

        public bool IsFile(string path)
        {
            return m_FileSystems.Any(x => x.IsFile(path));
        }

        public bool IsDirectory(string path)
        {
            return m_FileSystems.Any(x => x.IsDirectory(path));
        }

        public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
        {
            return m_FileSystems.SelectMany(x => x.GetFiles(path, extension, recursive));
        }

        public IEnumerable<string> GetDirectories(string path, bool recursive)
        {
            return m_FileSystems.SelectMany(x => x.GetDirectories(path, recursive));
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
            var fs = m_FileSystems.Last(x => x.IsFile(path)) ?? GetWritable();
            return fs.OpenRead(path);
        }

        public Stream OpenWrite(string path, BadWriteMode mode)
        {
            return GetWritable().OpenWrite(path, mode);
        }

        public string GetCurrentDirectory()
        {
            return GetWritable().GetCurrentDirectory();
        }

        public void SetCurrentDirectory(string path)
        {
            foreach (var fs in m_FileSystems) fs.SetCurrentDirectory(path);
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

        private BadVirtualFileSystem GetWritable()
        {
            return m_FileSystems.Last();
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