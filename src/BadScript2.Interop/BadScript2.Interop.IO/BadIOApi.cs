using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.IO;

public class BadIOApi : BadInteropApi
{
    public BadIOApi() : base("IO") { }

    private BadTable CreatePath()
    {
        BadTable t = new BadTable();

        t.SetFunction<string>("GetFileName", (ctx, s) => Path.GetFileName(s));
        t.SetFunction<string>("GetFileNameWithoutExtension", (ctx, s) => Path.GetFileNameWithoutExtension(s));
        t.SetFunction<string>("GetDirectoryName", (ctx, s) => Path.GetDirectoryName(s) ?? BadObject.Null);
        t.SetFunction<string>("GetExtension", (ctx, s) => Path.GetExtension(s));
        t.SetFunction<string>("GetFullPath", (ctx, s) => Path.GetFullPath(s));
        t.SetFunction<string>("GetTempPath", (ctx, s) => Path.GetTempPath());
        t.SetFunction<string>("GetTempFileName", (ctx, s) => Path.GetTempFileName());
        t.SetFunction<string, string>("ChangeExtension", (ctx, s, ext) => Path.ChangeExtension(s, ext));
        t.SetProperty(
            "Combine",
            new BadInteropFunction("Combine", Combine, new BadFunctionParameter("parts", false, false, true))
        );

        return t;
    }

    private BadObject Combine(BadObject[] arg)
    {
        return Path.Combine(arg.Select(x => x.ToString()!).ToArray());
    }

    private BadTable CreateDirectory()
    {
        BadTable t = new BadTable();

        t.SetFunction<string>(
            "CreateDirectory",
            (ctx, s) =>
            {
                Directory.CreateDirectory(s);

                return BadObject.Null;
            }
        );

        t.SetFunction<string>("Exists", s => Directory.Exists(s));
        t.SetFunction<string, bool>(
            "Delete",
            Directory.Delete
        );


        t.SetFunction<string, string>(
            "Move",
            Directory.Move
        );

        t.SetFunction("GetCurrentDirectory", () => Directory.GetCurrentDirectory());
        t.SetFunction<string>("SetCurrentDirectory", Directory.SetCurrentDirectory);
        t.SetFunction("GetStartupDirectory", () => AppDomain.CurrentDomain.BaseDirectory);

        t.SetFunction<string, string, bool>(
            "GetDirectories",
            (ctx, s, p, b) => new BadArray(
                Directory.GetDirectories(s, p, b ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Select(x => (BadObject)x)
                    .ToList()
            )
        );

        t.SetFunction<string, string, bool>(
            "GetFiles",
            (ctx, s, p, b) => new BadArray(
                Directory.GetFiles(s, p, b ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Select(x => (BadObject)x)
                    .ToList()
            )
        );

        return t;
    }

    private BadTable CreateFile()
    {
        BadTable t = new BadTable();

        t.SetFunction<string, string>(
            "WriteAllText",
            File.WriteAllText
        );
        t.SetFunction<string>(
            "ReadAllText",
            s => File.ReadAllText(s)
        );
        t.SetFunction<string>(
            "ReadAllTextAsync",
            s => new BadTask(BadTaskUtils.WaitForTask(File.ReadAllTextAsync(s)), "File.ReadAllTextAsync")
        );
        t.SetFunction<string, string>(
            "WriteAllTextAsync",
            (file, str) => new BadTask(BadTaskUtils.WaitForTask(File.WriteAllTextAsync(file, str)), "File.WriteAllTextAsync")
        );
        t.SetFunction<string>(
            "Exists",
            s => File.Exists(s)
        );
        t.SetFunction<string>(
            "ReadAllLines",
            s => new BadArray(
                File.ReadAllLines(s)
                    .Select(x => (BadObject)x)
                    .ToList()
            )
        );
        t.SetFunction<string, BadArray>(
            "WriteAllLines",
            (s, a) =>
            {
                File.WriteAllLines(s, a.InnerArray.Select(x => x.ToString()!));

                return BadObject.Null;
            }
        );

        t.SetFunction<string>(
            "ReadAllLinesAsync",
            s => new BadTask(BadTaskUtils.WaitForTask(File.ReadAllLinesAsync(s)), "File.ReadAllLinesAsync")
        );
        
        t.SetFunction<string, BadArray>(
            "WriteAllLinesAsync",
            (file, str) => new BadTask(
                BadTaskUtils.WaitForTask(File.WriteAllLinesAsync(file, str.InnerArray.Select(x => x.ToString()))),
                "File.WriteAllLinesAsync"
            )
        );

        return t;
    }

    public override void Load(BadTable target)
    {
        target.SetProperty("Path", CreatePath());
        target.SetProperty("Directory", CreateDirectory());
        target.SetProperty("File", CreateFile());
    }
}