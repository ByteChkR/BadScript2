using System.IO.Compression;
using System.Text;

using BadScript2.Common.Logging;
using BadScript2.IO.Virtual;

namespace BadScript2.IO;

/// <summary>
///     File System Helper Extensions
/// </summary>
public static class BadFileSystemHelper
{
    /// <summary>
    ///     Exports a Virtual File System to a Zip File
    /// </summary>
    /// <param name="fs">The Virtual File System to Export</param>
    /// <param name="str">The Stream to write the Zip File to</param>
    /// <param name="path">The Path to export</param>
    /// <exception cref="NotSupportedException">Gets thrown when the File System is not supported</exception>
    public static void ExportZip(this IVirtualFileSystem fs, Stream str, string path = "/")
    {
        BadLogger.Log("Exporting zip file..", "BFS");

        using ZipArchive zip = new ZipArchive(str, ZipArchiveMode.Update, true);

        foreach (string file in fs.GetFiles(path, "", true))
        {
            BadLogger.Log("Exporting File: " + file, "BFS");
            ZipArchiveEntry e = zip.CreateEntry(file.Remove(0, path.Length));
            using Stream estr = e.Open();
            using Stream fstr = fs.OpenRead(file);
            fstr.CopyTo(estr);
        }
    }

    /// <summary>
    /// Imports a Zip File to a Virtual File System
    /// </summary>
    /// <param name="fs">The Virtual File System to Import to</param>
    /// <param name="path">The Path to the Zip File</param>
    /// <param name="root">The directory that the zip file will be imported to</param>
    public static void ImportZip(this IVirtualFileSystem fs, string path, string root = "/")
    {
        using FileStream str = new FileStream(path, FileMode.Open, FileAccess.Read);
        fs.ImportZip(str, root);
    }

    /// <summary>
    ///     Imports a Zip File to a Virtual File System
    /// </summary>
    /// <param name="fs">The Virtual File System to Import to</param>
    /// <param name="str">The Stream to read the Zip File from</param>
    /// <param name="root">The directory that the zip file will be imported to</param>
    /// <exception cref="NotSupportedException">Gets thrown when the File System is not supported</exception>
    /// <exception cref="Exception">Gets thrown when the Zip File is invalid</exception>
    public static void ImportZip(this IVirtualFileSystem fs, Stream str, string root = "/", Encoding? encoding = null)
    {
        BadLogger.Log("Importing zip file..", "BFS");


        ZipArchive? arch = new ZipArchive(str, ZipArchiveMode.Read, false, encoding ?? Encoding.UTF8);

        if (arch == null)
        {
            throw new Exception("Failed to open zip file");
        }

        foreach (ZipArchiveEntry entry in arch.Entries)
        {
            BadLogger.Log("Importing File: " + entry.FullName, "BFS");

            if (BadVirtualPathReader.IsDirectory(root + entry.FullName))
            {
                continue;
            }

            string? dir = Path.GetDirectoryName(entry.FullName);

            if (dir != null && !string.IsNullOrEmpty(dir))
            {
                fs.CreateDirectory(root + dir, true);
            }

            using Stream s = entry.Open();
            using Stream o = fs.OpenWrite(root + entry.FullName, BadWriteMode.CreateNew);
            s.CopyTo(o);
        }

        arch.Dispose();
    }
}