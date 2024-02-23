using System.IO.Compression;

using BadScript2.IO;

namespace BadScript2.Interop.Compression;

[BadInteropApi("Zip", true)]
internal partial class BadZipApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem = BadFileSystem.Instance;


    /// <summary>
    ///     Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">File System Instance to use</param>
    public BadZipApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }

    /// <summary>
    ///     Decompresses the given file to the given output directory
    /// </summary>
    /// <param name="outputDir">Output Directory</param>
    /// <param name="inputFile">Input File</param>
    [BadMethod("ToDirectory", "Extracts a Zip Archive to the given directory")]
    private void ToDirectoryApi([BadParameter(description: "Output Directory")] string outputDir, [BadParameter(description: "Input File")] string inputFile)
    {
        using Stream s = m_FileSystem.OpenRead(inputFile);
        ToDirectory(outputDir, s);
    }

    /// <summary>
    ///     Compreses the given directory to the given output file
    /// </summary>
    /// <param name="inputDir">Input Directory</param>
    /// <param name="outputFile">Output File</param>
    [BadMethod("FromDirectory", "Creates a new Zip Archive from the given directory")]
    private void FromDirectoryApi([BadParameter(description: "Input Directory")] string inputDir, [BadParameter(description: "Output File")] string outputFile)
    {
        using Stream s = m_FileSystem.OpenWrite(outputFile, BadWriteMode.CreateNew);
        FromDirectory(inputDir, s);
    }

    /// <summary>
    ///     Decompresses the given stream to the given output directory
    /// </summary>
    /// <param name="outputDir">directory</param>
    /// <param name="input">Input Stream</param>
    private void ToDirectory(string outputDir, Stream input)
    {
        using ZipArchive archive = new ZipArchive(input, ZipArchiveMode.Read);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string outputFile = Path.Combine(outputDir, entry.FullName);
            string outputParent = Path.GetDirectoryName(outputFile)!;
            m_FileSystem.CreateDirectory(outputParent, true);
            using Stream s = m_FileSystem.OpenWrite(outputFile, BadWriteMode.CreateNew);
            using Stream e = entry.Open();
            e.CopyTo(s);
        }
    }

    /// <summary>
    ///     Compreses the given directory to the given output stream
    /// </summary>
    /// <param name="inputDir">Input Directory</param>
    /// <param name="output">Stream</param>
    private void FromDirectory(string inputDir, Stream output)
    {
        using ZipArchive archive = new ZipArchive(output, ZipArchiveMode.Create);
        string[] files = m_FileSystem.GetFiles(inputDir, "", true).ToArray();

        foreach (string file in files)
        {
            string zipPath = file.Remove(0, inputDir.Length + 1).Replace('\\', '/');
            ZipArchiveEntry entry = archive.CreateEntry(zipPath);
            using Stream es = entry.Open();
            using Stream s = m_FileSystem.OpenRead(file);
            s.CopyTo(es);
        }
    }
}