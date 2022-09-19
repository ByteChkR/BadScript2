using BadScript2.IO;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.IO;

public class BadIOApi : BadInteropApi
{
    public BadIOApi() : base("IO") { }

    private BadTable CreatePath()
    {
        BadTable t = new BadTable();

        t.SetFunction<string>("GetFileName", (_, s) => Path.GetFileName(s));
        t.SetFunction<string>("GetFileNameWithoutExtension", (_, s) => Path.GetFileNameWithoutExtension(s));
        t.SetFunction<string>("GetDirectoryName", (_, s) => Path.GetDirectoryName(s) ?? BadObject.Null);
        t.SetFunction<string>("GetExtension", (_, s) => Path.GetExtension(s));
        t.SetFunction<string>("GetFullPath", (_, s) => BadFileSystem.Instance.GetFullPath(s));
        t.SetFunction<string>("GetStartupPath", (_, _) => BadFileSystem.Instance.GetStartupDirectory());
        t.SetFunction<string, string>("ChangeExtension", (_, s, ext) => Path.ChangeExtension(s, ext));
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
            (_, s) =>
            {
                BadFileSystem.Instance.CreateDirectory(s);

                return BadObject.Null;
            }
        );

        t.SetFunction<string>("Exists", s => BadFileSystem.Instance.Exists(s) && BadFileSystem.Instance.IsDirectory(s));
        t.SetFunction<string, bool>(
            "Delete",
            BadFileSystem.Instance.DeleteDirectory
        );


        t.SetFunction<string, string, bool>(
            "Move",
            BadFileSystem.Instance.Move
        );

        t.SetFunction("GetCurrentDirectory", () => BadFileSystem.Instance.GetCurrentDirectory());
        t.SetFunction<string>("SetCurrentDirectory", BadFileSystem.Instance.SetCurrentDirectory);
        t.SetFunction("GetStartupDirectory", () => BadFileSystem.Instance.GetStartupDirectory());

        t.SetFunction<string, bool>(
            "GetDirectories",
            (_, s, b) => new BadArray(
                BadFileSystem.Instance.GetDirectories(s, b)
                    .Select(x => (BadObject)x)
                    .ToList()
            )
        );

        t.SetFunction<string, string, bool>(
            "GetFiles",
            (_, s, p, b) => new BadArray(
                BadFileSystem.Instance.GetFiles(s, p, b)
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
            BadFileSystem.WriteAllText
        );
        t.SetFunction<string>(
            "ReadAllText",
            s => BadFileSystem.ReadAllText(s)
        );
        t.SetFunction<string>(
            "Exists",
            s => BadFileSystem.Instance.Exists(s)
        );
        t.SetFunction<string>(
            "ReadAllLines",
            s => new BadArray(
                BadFileSystem.ReadAllLines(s)
                    .Select(x => (BadObject)x)
                    .ToList()
            )
        );
        t.SetFunction<string, BadArray>(
            "WriteAllLines",
            (s, a) =>
            {
                BadFileSystem.WriteAllLines(s, a.InnerArray.Select(x => x.ToString()!));

                return BadObject.Null;
            }
        );


        t.SetFunction<string, BadArray>(
            "WriteAllBytes",
            (s, a) =>
            {
                using Stream stream = BadFileSystem.Instance.OpenWrite(s, BadWriteMode.CreateNew);
                foreach (BadObject o in a.InnerArray)
                {
                    if (o is not IBadNumber num)
                    {
                        throw new BadRuntimeException("BadIO.WriteAllBytes: Array contains non-number");
                    }

                    stream.WriteByte((byte)num.Value);
                }
            }
        );

        t.SetFunction<string>(
            "ReadAllBytes",
            s =>
            {
                using Stream stream = BadFileSystem.Instance.OpenRead(s);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                return new BadArray(bytes.Select(x => (BadObject)new BadNumber(x)).ToList());
            }
        );

        t.SetFunction<string>("Delete", BadFileSystem.Instance.DeleteFile);

        return t;
    }

    public override void Load(BadTable target)
    {
        target.SetProperty("Path", CreatePath());
        target.SetProperty("Directory", CreateDirectory());
        target.SetProperty("File", CreateFile());
    }
}