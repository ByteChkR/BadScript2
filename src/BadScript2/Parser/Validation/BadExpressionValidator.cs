using BadScript2.Parser.Expressions;
/// <summary>
/// Contains the Comparison Operators for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Validation;

/// <summary>
/// Base class for all expression validators.
/// </summary>
public abstract class BadExpressionValidator
{
    /// <summary>
    /// Validates the given expression.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <param name="expr">The expression to validate.</param>
    public abstract void Validate(BadExpressionValidatorContext context, BadExpression expr);
}

/// <summary>
/// Base class for all expression validators that work with a specific expression type.
/// </summary>
/// <typeparam name="T">The expression type.</typeparam>
public abstract class BadExpressionValidator<T> : BadExpressionValidator
    where T : BadExpression
{
    /// <summary>
    /// Validates the given expression.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <param name="expr">The expression to validate.</param>
    protected abstract void Validate(BadExpressionValidatorContext context, T expr);

    /// <inheritdoc cref="BadExpressionValidator.Validate" />
    public override void Validate(BadExpressionValidatorContext context, BadExpression expr)
    {
        if (expr is T t)
        {
            Validate(context, t);
        }
    }
}