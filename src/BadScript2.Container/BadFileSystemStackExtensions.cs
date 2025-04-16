using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using Newtonsoft.Json.Linq;

namespace BadScript2.Container
{
    /// <summary>
    /// Extensions for the BadFileSystemStack
    /// </summary>
    public static class BadFileSystemStackExtensions
    {
        
        /// <summary>
        /// Creates a Zip Layer from a file
        /// </summary>
        /// <param name="stack">The File System Stack</param>
        /// <param name="file">The Zip File</param>
        /// <param name="root">The Mount Point</param>
        /// <param name="name">The Optional Name of the Layer</param>
        /// <returns>The File System Stack</returns>
        public static BadFileSystemStack FromZip(this BadFileSystemStack stack, string file, string root = "/", string? name = null)
        {
            return stack.ConfigureLayer(() =>
            {
                var fs = new BadVirtualFileSystem();
                fs.ImportZip(file, root);
                return BadLayeredFileSystemLayer.Create(name ?? $"Zip({file})", fs, new JObject
                {
                    ["File"] = file,
                    ["Root"] = root
                });
            });
        }
        /// <summary>
        /// Creates a Zip Layer from a stream
        /// </summary>
        /// <param name="stack">The File System Stack</param>
        /// <param name="stream">The Zip Stream</param>
        /// <param name="root">The Mount Point</param>
        /// <param name="name">The Optional Name of the Layer</param>
        /// <returns>The File System Stack</returns>
        public static BadFileSystemStack FromZip(this BadFileSystemStack stack, Stream stream, string root = "/", string? name = null)
        {
            return stack.ConfigureLayer(() =>
            {
                var fs = new BadVirtualFileSystem();
                fs.ImportZip(stream, root);
                return BadLayeredFileSystemLayer.Create(name ?? $"Zip(from Stream)", fs, new JObject
                {
                    ["File"] = null,
                    ["Root"] = root
                });
            });
        }

        /// <summary>
        /// Creates a Layer from a directory
        /// </summary>
        /// <param name="stack">The File System Stack</param>
        /// <param name="dir">The Directory</param>
        /// <param name="root">The Mount Point</param>
        /// <param name="name">The Optional Name of the Layer</param>
        /// <returns>The File System Stack</returns>
        public static BadFileSystemStack FromDirectory(this BadFileSystemStack stack, string dir, string root = "/", string? name = null)
        {
            return stack.ConfigureLayer(() =>
            {
                var fs = new BadVirtualFileSystem();
                fs.SetCurrentDirectory(root);

                var fp = Path.GetFullPath(dir);
                if (!fp.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    fp += Path.DirectorySeparatorChar;
                }

                foreach (var file in Directory.GetFiles(fp, "*", SearchOption.AllDirectories))
                {
                    var relFile = file.Replace(fp, "");
                    using var s = File.OpenRead(file);
                    using var d = fs.OpenWrite(relFile, BadWriteMode.CreateNew);
                    s.CopyTo(d);
                }

                return BadLayeredFileSystemLayer.Create(name ?? $"Directory({fp})", fs, new JObject
                {
                    ["Directory"] = fp,
                    ["Root"] = root
                });
            });
        }

        /// <summary>
        /// Creates a Layer from a web archive
        /// </summary>
        /// <param name="stack">The File System Stack</param>
        /// <param name="url">The URL of the web archive</param>
        /// <param name="root">The Mount Point</param>
        /// <param name="name">The Optional Name of the Layer</param>
        /// <returns>The File System Stack</returns>
        public static async Task<BadFileSystemStack> FromWebArchive(this BadFileSystemStack stack, string url, string root = "/", string? name = null)
        {
            using var http = new HttpClient();
            return stack.FromZip(await http.GetStreamAsync(url), root, name ?? $"WebArchive({url})");
        }
    }
}