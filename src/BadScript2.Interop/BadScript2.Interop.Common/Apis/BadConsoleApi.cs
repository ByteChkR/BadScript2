using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Common.Apis
{
    public class BadConsoleApi : BadInteropApi
    {
        public BadConsoleApi() : base("Console") { }

        public override void Load(BadTable target)
        {
            target.SetFunction<BadObject>("WriteLine", WriteLine);
            target.SetFunction<BadObject>("Write", Write);
            target.SetFunction("Clear", Clear);
            target.SetFunction("ReadLine", () => Console.ReadLine());
        }

        private void Write(BadObject obj)
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

        private void WriteLine(BadObject obj)
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

        public void Clear()
        {
            Console.Clear();
        }
    }
}