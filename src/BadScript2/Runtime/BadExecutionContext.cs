using BadScript2.Parser.Expressions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime;

public class BadExecutionContext
{
    public readonly BadScope Scope;

    public BadExecutionContext(BadScope scope)
    {
        Scope = scope;
    }

    public static BadExecutionContext Create()
    {
        return new BadExecutionContext(new BadScope("<root>"));
    }


    public BadObject? Run(IEnumerable<BadExpression> expressions)
    {
        foreach (BadObject o in Execute(expressions))
        {
            //Execute
        }

        if (Scope.ReturnValue != null)
        {
            return Scope.ReturnValue;
        }

        return null;
    }

    public IEnumerable<BadObject> Execute(IEnumerable<BadExpression> expressions)
    {
        foreach (BadExpression expression in expressions)
        {
            foreach (BadObject o in expression.Execute(this))
            {
                yield return o;

                if (Scope.ReturnValue != null ||
                    Scope.IsBreak ||
                    Scope.IsContinue ||
                    Scope.IsError)
                {
                    yield break;
                }
            }
        }
    }
}