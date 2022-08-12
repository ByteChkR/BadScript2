using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Common.Apis;

public class BadConsoleApi : BadInteropApi
{
    public static Action<BadObject> OnWrite = Write;
    public static Action<BadObject> OnWriteLine = WriteLine;
    public static Action OnClear = Clear;
    public static Func<string> OnReadLine = ReadLine;
    public BadConsoleApi() : base("Console") { }

    public static bool AllowInput { get; set; } = true;

    public override void Load(BadTable target)
    {
        target.SetFunction("WriteLine", OnWriteLine);
        target.SetFunction("Write", OnWrite);
        target.SetFunction("Clear", OnClear);
        target.SetFunction("ReadLine", () => OnReadLine());
    }

    private static void Write(BadObject obj)
    {
        if (obj is IBadString str)
        {
            Console.Write(str.Value);
        }
        else
        {
            Console.Write(obj);
        }
    }

    private static void WriteLine(BadObject obj)
    {
        if (obj is IBadString str)
        {
            Console.WriteLine(str.Value);
        }
        else
        {
            Console.WriteLine(obj);
        }
    }

    public static void Clear()
    {
        Console.Clear();
    }

    public static string ReadLine()
    {
        if (!AllowInput)
        {
            throw new Exception("Input is not allowed");
        }

        return Console.ReadLine() ?? "";
    }
}