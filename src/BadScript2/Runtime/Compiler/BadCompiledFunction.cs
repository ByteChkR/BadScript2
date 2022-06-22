using System.Text;

using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.Compiler
{
    public class BadCompiledFunction : BadFunction
    {
        public readonly BadCompilerResult CompilerResult;
        public readonly int Entry;
        private readonly int m_Length;
        private readonly BadSourcePosition m_Position;
        public readonly BadScope ParentScope;

        public BadCompiledFunction(
            BadWordToken? name,
            BadScope parentScope,
            BadSourcePosition position,
            BadCompilerResult compilerResult,
            int entry,
            int length,
            params BadFunctionParameter[] parameters) : base(name, parameters)
        {
            ParentScope = parentScope;
            m_Position = position;
            CompilerResult = compilerResult;
            Entry = entry;
            m_Length = length;
        }

        public static IEnumerable<BadObject> Invoke(
            BadCompiledFunction func,
            BadObject[] args,
            BadExecutionContext ctx)
        {
            func.ApplyParameters(ctx, args, func.m_Position);

            BadVirtualMachine vm = new BadVirtualMachine(func.CompilerResult, func.Entry);
            foreach (BadObject o in vm.Execute(ctx))
            {
                yield return o;
            }
        }

        protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
        {
            BadExecutionContext ctx = new BadExecutionContext(
                ParentScope.CreateChild(
                    ToString(),
                    caller.Scope,
                    BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
                )
            );
            foreach (BadObject o in Invoke(this, args, ctx))
            {
                yield return o;
            }
        }

        public string GetBodyString()
        {
            StringBuilder sb = new StringBuilder();
            BadInstruction[] instrs = CompilerResult.GetInstructions();
            sb.AppendLine("");
            sb.AppendLine("{");
            for (int i = Entry; i < Entry + m_Length; i++)
            {
                BadInstruction inst = instrs[i];
                sb.AppendLine($"\t{i}: {inst}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToSafeString(new List<BadObject>());
        }

        public override string ToSafeString(List<BadObject> done)
        {
            string header = "<compiled> " + GetHeader() + " at " + m_Position.GetPositionInfo();
            header += GetBodyString();

            return header;
        }
    }
}