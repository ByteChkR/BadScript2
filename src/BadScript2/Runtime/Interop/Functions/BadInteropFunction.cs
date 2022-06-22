using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.Interop.Functions
{
    public class BadInteropFunction : BadFunction
    {
        private readonly Func<BadObject[], BadObject> m_Func;

        public BadInteropFunction(
            BadWordToken? name,
            Func<BadObject[], BadObject> func,
            params BadFunctionParameter[] parameters) : base(name, parameters)
        {
            m_Func = func;
        }


        public static BadInteropFunction Create(Func<BadObject[], BadObject> func, params string[] names)
        {
            BadInteropFunction function = new BadInteropFunction(
                null,
                func,
                names.Select(x => (BadFunctionParameter)x).ToArray()
            );

            return function;
        }


        protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
        {
            CheckParameters(args);

            yield return m_Func.Invoke(args);
        }

        public override string ToSafeString(List<BadObject> done)
        {
            return "<interop> " + base.ToSafeString(done);
        }
    }
}