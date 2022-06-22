using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interactive
{
    public class BadInteractiveConsoleApi : BadInteropApi
    {
        private readonly BadInteractiveConsole m_Console;

        public BadInteractiveConsoleApi(BadInteractiveConsole console) : base("Interactive")
        {
            m_Console = console;
        }

        public override void Load(BadTable target)
        {
            target.SetFunction("Reset", m_Console.Reset);
            target.SetFunction<string>("Run", m_Console.Run);
            target.SetFunction<string>("Load", m_Console.Load);
            target.SetFunction<string>("RunIsolated", m_Console.RunIsolated);
            target.SetFunction<string>("LoadIsolated", m_Console.LoadIsolated);
            target.SetFunction("GetScope", GetScope);
            target.SetFunction<bool>("SetCatchError", SetCatchError);
            target.SetFunction<bool>("SetPreParse", SetPreParse);
        }

        private void SetCatchError(bool enable)
        {
            m_Console.CatchErrors = enable;
        }

        private void SetPreParse(bool enable)
        {
            m_Console.PreParse = enable;
        }

        private BadObject GetScope()
        {
            return m_Console.CurrentScope ?? BadObject.Null;
        }
    }
}