using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Validation;

public abstract class BadExpressionValidator
{
    public abstract void Validate(BadExpressionValidatorContext context, BadExpression expr);
}

public abstract class BadExpressionValidator<T> : BadExpressionValidator
    where T : BadExpression
{
    protected abstract void Validate(BadExpressionValidatorContext context, T expr);

    public override void Validate(BadExpressionValidatorContext context, BadExpression expr)
    {
        if (expr is T t)
        {
            Validate(context, t);
        }
    }
}