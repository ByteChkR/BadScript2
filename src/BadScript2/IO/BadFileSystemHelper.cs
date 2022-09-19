using System.IO.Compression;

using BadScript2.Common.Logging;
using BadScript2.IO.Virtual;

namespace BadScript2.IO;

/// <summary>
/// File System Helper Extensions
/// </summary>
public static class BadFileSystemHelper
{
    /// <summary>
    /// Exports a Virtual File System to a Zip File
    /// </summary>
    /// <param name="fs">The Virtual File System to Export</param>
    /// <param name="str">The Stream to write the Zip File to</param>
    /// <exception cref="NotSupportedException">Gets thrown when the File System is not supported</exception>
    public static void ExportZip(this IFileSystem fs, Stream str)
    {
        BadLogger.Log("Exporting zip file..", "BFS");
        if (fs is not BadVirtualFileSystem vfs)
        {
            throw new NotSupportedException("Only virtual file systems are supported");
        }

        using ZipArchive zip = new ZipArchive(str, ZipArchiveMode.Update, true);
        foreach (string file in vfs.GetFiles("/", "", true))
        {
            BadLogger.Log("Exporting File: " + file, "BFS");
            ZipArchiveEntry e = zip.CreateEntry(file.Remove(0, 1));
            using Stream estr = e.Open();
            using Stream fstr = vfs.OpenRead(file);
            fstr.CopyTo(estr);
        }

        zip.Dispose();
    }

    /// <summary>
    /// Imports a Zip File to a Virtual File System
    /// </summary>
    /// <param name="fs">The Virtual File System to Import to</param>
    /// <param name="str">The Stream to read the Zip File from</param>
    /// <exception cref="NotSupportedException">Gets thrown when the File System is not supported</exception>
    /// <exception cref="Exception">Gets thrown when the Zip File is invalid</exception>
    public static void ImportZip(this IFileSystem fs, Stream str)
    {
        BadLogger.Log("Importing zip file..", "BFS");
        if (fs is not BadVirtualFileSystem vfs)
        {
            throw new NotSupportedException("Only virtual file systems are supported");
        }

        ZipArchive? arch = new ZipArchive(str, ZipArchiveMode.Read);
        if (arch == null)
        {
            throw new Exception("Failed to open zip file");
        }

        foreach (ZipArchiveEntry entry in arch.Entries)
        {
            BadLogger.Log("Importing File: " + entry.FullName, "BFS");
            if (BadVirtualPathReader.IsDirectory(entry.FullName))
            {
                continue;
            }

            string? dir = Path.GetDirectoryName(entry.FullName);
            if (dir != null && !string.IsNullOrEmpty(dir))
            {
                vfs.CreateDirectory(dir, true);
            }

            using Stream s = entry.Open();
            using Stream o = vfs.OpenWrite(entry.FullName, BadWriteMode.CreateNew);
            s.CopyTo(o);
        }

        arch.Dispose();
    }
}