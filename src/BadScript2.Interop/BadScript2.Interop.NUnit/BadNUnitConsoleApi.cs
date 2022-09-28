using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.NUnit
{
    public class BadNUnitConsoleApi : BadInteropApi
    {
        private readonly BadUnitTestContextBuilder m_Console;

        public BadNUnitConsoleApi(BadUnitTestContextBuilder console) : base("NUnit")
        {
            m_Console = console;
        }

        public override void Load(BadTable target)
        {
            target.SetFunction<string>("Load", s => m_Console.Register(false, false, s));
            target.SetFunction("Reset", m_Console.Reset);
            target.SetFunction<BadFunction, BadObject>("AddTest", m_Console.AddTest);
            target.SetFunction<BadFunction>("AddSetup", m_Console.AddSetup);
            target.SetFunction<BadFunction>("AddTeardown", m_Console.AddTeardown);
        }
    }
}