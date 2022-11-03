using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Common.Apis
{
    public class BadConsoleApi : BadInteropApi
    {
        private readonly IBadConsole? m_Console;
        public BadConsoleApi() : this(BadConsole.GetConsole()) { }

        public BadConsoleApi(IBadConsole console) : base("Console")
        {
            m_Console = console;
            OnWrite = Write;
            OnWriteLine = WriteLine;
            OnClear = Clear;
            OnReadLine = ReadLine;
            OnReadLineAsync = ReadLineAsync;
        }

        private IBadConsole Console => m_Console ?? BadConsole.GetConsole();
        public Action<BadObject> OnWrite { get; set; }
        public Action<BadObject> OnWriteLine { get; set; }
        public Action OnClear { get; set; }
        public Func<string> OnReadLine { get; set; }
        public Func<Task<string>> OnReadLineAsync { get; set; }

        public bool AllowInput { get; set; } = true;

        private Task<string> ReadLineAsync()
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
                () => new BadTask(new ReadLineAsyncRunnable(ReadLineAsyncBlocking().GetEnumerator()), "Console.ReadLineAsync")
            );
        }

        private IEnumerable<BadObject> ReadLineAsyncBlocking()
        {
            Task<string>? t = OnReadLineAsync();

            while (!t.IsCompleted)
            {
                yield return BadObject.Null;
            }

            yield return t.Result;
        }

        private void Write(BadObject obj)
        {
            if (obj is IBadString str)
            {
                Console.Write(str.Value);
            }
            else
            {
                Console.Write(obj.ToString());
            }
        }

        private void WriteLine(BadObject obj)
        {
            if (obj is IBadString str)
            {
                Console.WriteLine(str.Value);
            }
            else
            {
                Console.WriteLine(obj.ToString());
            }
        }

        public void Clear()
        {
            Console.Clear();
        }

        public string ReadLine()
        {
            if (!AllowInput)
            {
                throw new Exception("Input is not allowed");
            }

            return Console.ReadLine() ?? "";
        }

        private class ReadLineAsyncRunnable : BadRunnable
        {
            private readonly IEnumerator<BadObject> m_Task;
            private BadObject m_Return = BadObject.Null;

            public ReadLineAsyncRunnable(IEnumerator<BadObject> task)
            {
                m_Task = task;
            }

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
}