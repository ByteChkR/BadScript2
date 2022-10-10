using System.IO.Compression;
using System.Text;

using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Compression
{
    public class BadCompressionApi : BadInteropApi
    {
        private readonly IFileSystem m_FileSystem;

        public BadCompressionApi() : this(BadFileSystem.Instance) { }

        public BadCompressionApi(IFileSystem fileSystem) : base("Compression")
        {
            m_FileSystem = fileSystem;
        }


        private static BadObject Deflate(IBadString obj)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj.Value));
            MemoryStream compressed = new MemoryStream();
            using DeflateStream ds = new DeflateStream(compressed, CompressionMode.Compress);
            ms.CopyTo(ds);
            ds.Close();

            return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
        }

        private static BadObject Inflate(BadArray obj)
        {
            MemoryStream ms = new MemoryStream(obj.InnerArray.Select(x => (byte)((BadNumber)x).Value).ToArray());
            MemoryStream decompressed = new MemoryStream();
            using DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress);
            ds.CopyTo(decompressed);
            ds.Close();

            return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
        }

        private static BadObject GZipCompress(IBadString obj)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj.Value));
            MemoryStream compressed = new MemoryStream();
            using GZipStream ds = new GZipStream(compressed, CompressionMode.Compress);
            ms.CopyTo(ds);
            ds.Close();

            return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
        }

        private static BadObject GZipDecompress(BadArray obj)
        {
            MemoryStream ms = new MemoryStream(obj.InnerArray.Select(x => (byte)((BadNumber)x).Value).ToArray());
            MemoryStream decompressed = new MemoryStream();
            using GZipStream ds = new GZipStream(ms, CompressionMode.Decompress);
            ds.CopyTo(decompressed);
            ds.Close();

            return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
        }

        private static BadObject ZLibCompress(IBadString obj)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(obj.Value));
            MemoryStream compressed = new MemoryStream();
            using ZLibStream ds = new ZLibStream(compressed, CompressionMode.Compress);
            ms.CopyTo(ds);
            ds.Close();

            return new BadArray(compressed.ToArray().Select(x => (BadObject)new BadNumber(x)).ToList());
        }

        private static BadObject ZLibDecompress(BadArray obj)
        {
            MemoryStream ms = new MemoryStream(obj.InnerArray.Select(x => (byte)((BadNumber)x).Value).ToArray());
            MemoryStream decompressed = new MemoryStream();
            using ZLibStream ds = new ZLibStream(ms, CompressionMode.Decompress);
            ds.CopyTo(decompressed);
            ds.Close();

            return new BadString(Encoding.UTF8.GetString(decompressed.ToArray()));
        }

        public override void Load(BadTable target)
        {
            BadTable deflate = new BadTable();
            deflate.SetFunction<IBadString>("Compress", Deflate);
            deflate.SetFunction<BadArray>("Decompress", Inflate);
            target.SetProperty("Deflate", deflate);

            BadTable gzip = new BadTable();
            gzip.SetFunction<IBadString>("Compress", GZipCompress);
            gzip.SetFunction<BadArray>("Decompress", GZipDecompress);
            target.SetProperty("GZip", gzip);

            BadTable zlib = new BadTable();
            zlib.SetFunction<IBadString>("Compress", ZLibCompress);
            zlib.SetFunction<BadArray>("Decompress", ZLibDecompress);
            target.SetProperty("ZLib", zlib);

            BadTable zip = new BadTable();
            zip.SetFunction<string, string>("FromDirectory", FromDirectory);
            zip.SetFunction<string, string>("ToDirectory", ToDirectory);
            target.SetProperty("Zip", zip);
        }

        private void ToDirectory(BadExecutionContext ctx, string outputDir, string inputFile)
        {
            using Stream s = m_FileSystem.OpenRead(inputFile);
            ToDirectory(outputDir, s);
        }

        private void FromDirectory(BadExecutionContext ctx, string inputDir, string outputFile)
        {
            using Stream s = m_FileSystem.OpenWrite(outputFile, BadWriteMode.CreateNew);
            FromDirectory(inputDir, s);
        }

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

        private void FromDirectory(string inputDir, Stream output)
        {
            using ZipArchive archive = new ZipArchive(output, ZipArchiveMode.Create);
            string[] files = m_FileSystem.GetFiles(inputDir, "", true).ToArray();
            foreach (string file in files)
            {
                string zipPath = file.Remove(0, inputDir.Length+1).Replace('\\', '/');
                ZipArchiveEntry entry = archive.CreateEntry(zipPath);
                using Stream es = entry.Open();
                using Stream s = m_FileSystem.OpenRead(file);
                s.CopyTo(es);
            }
        }
    }
}