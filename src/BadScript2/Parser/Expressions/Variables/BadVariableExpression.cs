using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Variables;

/// <summary>
///     Implements the Variable Expression
/// </summary>
public class BadVariableExpression : BadExpression
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

    /// <summary>
    ///     Returns the String representation of the Variable Expression
    /// </summary>
    /// <returns>String Representation</returns>
    public override string ToString()
	{
		return Name;
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject name = BadObject.Wrap(Name);

		BadObjectReference obj = context.Scope.GetVariable(name, context.Scope);


		yield return obj;
	}
}
