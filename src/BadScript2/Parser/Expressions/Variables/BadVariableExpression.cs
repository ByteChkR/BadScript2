using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

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
    public BadVariableExpression(string name, BadSourcePosition position, params BadExpression[] genericParameters) :
        base(false, position)
    {
        Name = name;
        GenericParameters = genericParameters;
    }

    /// <summary>
    ///     Name of the Variable
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// List of Generic Parameters
    /// </summary>
    public IReadOnlyList<BadExpression> GenericParameters { get; }

#region IBadNamedExpression Members

    /// <inheritdoc cref="IBadNamedExpression.GetName" />
    public string? GetName()
    {
        return Name;
    }

#endregion

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
        if (!context.Scope.HasVariable(Name, context.Scope))
        {
            throw BadRuntimeException.Create(context.Scope, $"Variable '{Name}' is not defined", Position);
        }

        BadObjectReference obj = context.Scope.GetVariable(Name, context.Scope);

        if (GenericParameters.Count != 0)
        {
            if (obj.Dereference(Position) is not IBadGenericObject genType)
            {
                throw BadRuntimeException.Create(context.Scope, "Type is not generic", Position);
            }

            BadObject[] genParams = new BadObject[GenericParameters.Count];

            for (int i = 0; i < GenericParameters.Count; i++)
            {
                foreach (BadObject? o in GenericParameters[i]
                             .Execute(context))
                {
                    genParams[i] = o;
                }

                genParams[i] = genParams[i]
                    .Dereference(Position);
            }

            yield return genType.CreateGeneric(genParams);
        }
        else
        {
            yield return obj;
        }
    }
}