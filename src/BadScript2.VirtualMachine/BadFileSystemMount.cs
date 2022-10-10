using System.IO;

using BadScript2.IO;
using BadScript2.IO.Virtual;

namespace BadScript2.VirtualMachine
{
    public class BadFileSystemMount
    {
        public string MountPoint { get; set; } = "/";
        public string DataArchive { get; set; } = "data.zip";
        public bool IsPersistent { get; set; }

        public void Mount(BadVirtualFileSystem fs)
        {
            //fs.CreateDirectory(MountPoint, true);

            if (!File.Exists(DataArchive))
            {
                return;
            }

            using (Stream s = File.OpenRead(DataArchive))
            {
                fs.ImportZip(s);
            }
        }

        public void Unmount(BadVirtualFileSystem fs)
        {
            if (IsPersistent && fs.IsDirectory(MountPoint))
            {
                using (Stream s = File.Create(DataArchive))
                {
                    fs.ExportZip(s, MountPoint);
                }
            }
        }
    }
}