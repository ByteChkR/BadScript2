using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Variables;

/// <summary>
///     Implements the Variable Expression
/// </summary>
public class BadVariableExpression : BadExpression, IBadNamedExpression
{
	/// <summary>
	///     Constructor of the Variable Expression
	/// </summary>
	/// <param name="name">Name of the Variable</param>
	/// <param name="position">Source Position of the Expression</param>
	public BadVariableExpression(string name, BadSourcePosition position) : base(false, position)
    {
        Name = name;
    }

	/// <summary>
	///     Name of the Variable
	/// </summary>
	public string Name { get; }

    /// <inheritdoc cref="IBadNamedExpression.GetName" />
    public string? GetName()
    {
        return Name;
    }

    /// <summary>
    ///     Returns the String representation of the Variable Expression
    /// </summary>
    /// <returns>String Representation</returns>
    public override string ToString()
    {
        return Name;
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield break;
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {

        BadObjectReference obj = context.Scope.GetVariable(Name, context.Scope);


        yield return obj;
    }
}