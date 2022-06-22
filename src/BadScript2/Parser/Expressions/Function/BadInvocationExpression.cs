using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Expressions.Function
{
    public class BadInvocationExpression : BadExpression
    {
        public readonly BadExpression[] Arguments;

        public BadInvocationExpression(BadExpression left, BadExpression[] args, BadSourcePosition position) : base(
            false,
            false,
            position
        )
        {
            Left = left;
            Arguments = args;
        }

        public BadExpression Left { get; }

        public override void Optimize()
        {
            for (int i = 0; i < Arguments.Length; i++)
            {
                Arguments[i] = BadExpressionOptimizer.Optimize(Arguments[i]);
            }
        }

        public IEnumerable<BadObject> GetArgs(BadExecutionContext context, List<BadObject> args)
        {
            foreach (BadExpression argExpr in Arguments)
            {
                BadObject argObj = BadObject.Null;
                foreach (BadObject arg in argExpr.Execute(context))
                {
                    argObj = arg;

                    if (context.Scope.IsError)
                    {
                        yield break;
                    }

                    yield return arg;
                }

                args.Add(argObj.Dereference());
            }
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            BadObject left = BadObject.Null;
            foreach (BadObject o in Left.Execute(context))
            {
                left = o;

                yield return o;
            }

            if (context.Scope.IsError)
            {
                yield break;
            }

            left = left.Dereference();

            List<BadObject> args = new List<BadObject>();
            foreach (BadObject o in GetArgs(context, args))
            {
                yield return o;
            }

            if (context.Scope.IsError)
            {
                yield break;
            }

            if (left is BadFunction func)
            {
                foreach (BadObject o in func.Invoke(args.ToArray(), context))
                {
                    yield return o;
                }
            }
            else if (left.HasProperty(BadStaticKeys.InvocationOperatorName))
            {
                BadFunction? invocationOp =
                    left.GetProperty(BadStaticKeys.InvocationOperatorName).Dereference() as BadFunction;
                if (invocationOp == null)
                {
                    throw new BadRuntimeException("Function Invocation Operator is not a function", Position);
                }

                BadObject r = BadObject.Null;
                foreach (BadObject o in invocationOp.Invoke(args.ToArray(), context))
                {
                    yield return o;
                    r = o;
                }

                yield return r.Dereference();
            }
            else
            {
                throw new BadRuntimeException(
                    "Cannot invoke non-function object",
                    Position
                );
            }
        }
    }
}