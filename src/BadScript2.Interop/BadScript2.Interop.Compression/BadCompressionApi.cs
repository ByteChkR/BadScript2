using System.IO.Compression;
using System.Text;

using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Compression
{
    public class BadCompressionApi : BadInteropApi
    {
        public BadCompressionApi() : base("Compression") { }


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
        }
    }
}