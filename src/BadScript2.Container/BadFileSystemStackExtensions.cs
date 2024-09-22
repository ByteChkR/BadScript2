using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BadScript2.IO;
using BadScript2.IO.Virtual;

namespace BadScript2.Container
{
    public static class BadFileSystemStackExtensions
    {
        
        public static BadFileSystemStack FromZip(this BadFileSystemStack stack, string file, string root = "/")
        {
            return stack.ConfigureLayer(() =>
            {
                var fs = new BadVirtualFileSystem();
                fs.ImportZip(file, root);
                return fs;
            });
        }
        public static BadFileSystemStack FromZip(this BadFileSystemStack stack, Stream stream, string root = "/")
        {
            return stack.ConfigureLayer(() =>
            {
                var fs = new BadVirtualFileSystem();
                fs.ImportZip(stream, root);
                return fs;
            });
        }

        public static BadFileSystemStack FromDirectory(this BadFileSystemStack stack, string dir, string root = "/")
        {
            return stack.ConfigureLayer(() =>
            {
                var fs = new BadVirtualFileSystem();
                fs.SetCurrentDirectory(root);

                var fp = Path.GetFullPath(dir);
                if (!fp.EndsWith(Path.DirectorySeparatorChar))
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

                return fs;
            });
        }

        public static async Task<BadFileSystemStack> FromWebArchive(this BadFileSystemStack stack, string url, string root = "/")
        {
            using var http = new HttpClient();
            return stack.FromZip(await http.GetStreamAsync(url), root);
        }
    }
}