using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Common.Apis;

public class BadConsoleApi : BadInteropApi
{
    public BadConsoleApi() : base("Console") { }
    public static Action<BadObject> OnWrite { get; set; } = Write;
    public static Action<BadObject> OnWriteLine { get; set; } = WriteLine;
    public static Action OnClear { get; set; } = Clear;
    public static Func<string> OnReadLine { get; set; } = ReadLine;
    public static Func<Task<string>> OnReadLineAsync { get; set; } = ReadLineAsync;

    public static bool AllowInput { get; set; } = true;

    private static Task<string> ReadLineAsync()
    {
        return System.Threading.Tasks.Task.Run(Console.ReadLine);
    }

    public override void Load(BadTable target)
    {
        target.SetFunction("WriteLine", OnWriteLine);
        target.SetFunction("Write", OnWrite);
        target.SetFunction("Clear", OnClear);
        target.SetFunction("ReadLine", () => OnReadLine());
        target.SetFunction(
            "ReadLineAsync",
            () => new BadTask(new ReadLineAsyncRunnable(), "Console.ReadLineAsync")
        );
    }

    private static IEnumerable<BadObject> ReadLineAsyncBlocking()
    {
        Task<string>? t = OnReadLineAsync();

        while (!t.IsCompleted)
        {
            yield return BadObject.Null;
        }

        yield return t.Result;
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

    private class ReadLineAsyncRunnable : BadRunnable
    {
        private BadObject m_Return = BadObject.Null;
        private readonly IEnumerator<BadObject> m_Task = ReadLineAsyncBlocking().GetEnumerator();

        public override IEnumerator<BadObject> Enumerator
        {
            get
            {
                while (m_Task.MoveNext())
                {
                    m_Return = m_Task.Current!;

                    yield return m_Return;
                }
            }
        }

        public override BadObject GetReturn()
        {
            return m_Return;
        }
    }
}