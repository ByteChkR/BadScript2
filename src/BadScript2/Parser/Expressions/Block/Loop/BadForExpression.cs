using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block.Loop;

public class BadForExpression : BadExpression
{
    public readonly BadExpression[] Body;

    public BadForExpression(
        BadExpression varDef,
        BadExpression condition,
        BadExpression varIncrement,
        BadExpression[] body,
        BadSourcePosition position) : base(false, false, position)
    {
        VarDef = varDef;
        Condition = condition;
        VarIncrement = varIncrement;
        Body = body;
    }

    public BadExpression Condition { get; private set; }
    public BadExpression VarDef { get; private set; }
    public BadExpression VarIncrement { get; private set; }

    public override void Optimize()
    {
        Condition = BadExpressionOptimizer.Optimize(Condition);
        VarDef = BadExpressionOptimizer.Optimize(VarDef);
        VarIncrement = BadExpressionOptimizer.Optimize(VarIncrement);
        for (int i = 0; i < Body.Length; i++)
        {
            Body[i] = BadExpressionOptimizer.Optimize(Body[i]);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadExecutionContext loopCtx = new BadExecutionContext(context.Scope.CreateChild("ForLoop", context.Scope));

        foreach (BadObject o in VarDef.Execute(loopCtx))
        {
            yield return o;
        }


        BadObject cond = BadObject.Null;
        foreach (BadObject o in Condition.Execute(loopCtx))
        {
            cond = o;

            yield return o;
        }

        IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
                           throw new BadRuntimeException("While Condition is not a boolean", Position);

        while (bRet.Value)
        {
            BadExecutionContext loopContext = new BadExecutionContext(
                loopCtx.Scope.CreateChild(
                    "InnerForLoop",
                    loopCtx.Scope,
                    BadScopeFlags.Breakable | BadScopeFlags.Continuable
                )
            );
            foreach (BadObject o in loopContext.Execute(Body))
            {
                yield return o;
            }

            if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null || loopContext.Scope.IsError)
            {
                break;
            }

            foreach (BadObject o in VarIncrement.Execute(loopContext))
            {
                yield return o;
            }

            foreach (BadObject o in Condition.Execute(loopContext))
            {
                cond = o;

                yield return o;
            }

            bRet = cond.Dereference() as IBadBoolean ??
                   throw new BadRuntimeException("While Condition is not a boolean", Position);
        }

        yield return BadObject.Null;
    }
}