using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Console.Debugging.Scriptable
{
    public class BadScriptDebuggerApi : BadInteropApi
    {
        private readonly BadScriptDebugger m_Debugger;

        public BadScriptDebuggerApi(BadScriptDebugger debugger) : base("Debugger")
        {
            m_Debugger = debugger;
        }

        public void RegisterStep(BadExecutionContext context, BadFunction func)
        {
            m_Debugger.OnStep += s =>
            {
                foreach (BadObject o in func.Invoke(new[] { BadObject.Wrap(s) }, context)) { }
            };
        }

        public void RegisterOnFileLoaded(BadExecutionContext context, BadFunction func)
        {
            m_Debugger.OnFileLoaded += s =>
            {
                foreach (BadObject o in func.Invoke(new BadObject[] { s }, context)) { }
            };
        }

        public override void Load(BadTable target)
        {
            target.SetFunction<BadFunction>("RegisterStep", RegisterStep);
            target.SetFunction<BadFunction>("RegisterOnFileLoaded", RegisterOnFileLoaded);
        }
    }
}