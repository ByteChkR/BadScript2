using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Variables;

/// <summary>
///     Implements the Variable Definition Expression
/// </summary>
public class BadVariableDefinitionExpression : BadVariableExpression
{
    /// <summary>
    ///     Constructor of the Variable Definition Expression
    /// </summary>
    /// <param name="name">The name of the Variable</param>
    /// <param name="position">The Source Position of the Expression</param>
    /// <param name="typeExpression">The (optional) Type of the Variable</param>
    /// <param name="isReadOnly">Indicates if the Variable will be declared as Read-Only</param>
    public BadVariableDefinitionExpression(
        string name,
        BadSourcePosition position,
        BadExpression? typeExpression = null,
        bool isReadOnly = false) : base(
        name,
        position
    )
    {
        IsReadOnly = isReadOnly;
        TypeExpression = typeExpression;
    }

    /// <summary>
    ///     Indicates if the Variable will be declared as Read-Only
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    ///     The (optional) Type of the Variable
    /// </summary>
    public BadExpression? TypeExpression { get; }

    /// <summary>
    ///     String Representation of the Expression
    /// </summary>
    /// <returns>String Representation</returns>
    public override string ToString()
    {
        if (IsReadOnly)
        {
            return $"{BadStaticKeys.ConstantDefinitionKey} {Name}";
        }

        return $"{BadStaticKeys.VariableDefinitionKey} {Name}";
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadClassPrototype? type = null;
        if (TypeExpression != null)
        {
            BadObject obj = BadObject.Null;
            foreach (BadObject o in TypeExpression.Execute(context))
            {
                obj = o;

                yield return o;
            }

            if (context.Scope.IsError)
            {
                yield break;
            }

            obj = obj.Dereference();

            if (obj is not BadClassPrototype proto)
            {
                throw new BadRuntimeException("Type expression must be a class prototype", Position);
            }

            type = proto;
        }

        context.Scope.DefineVariable(BadObject.Wrap(Name), BadObject.Null, new BadPropertyInfo(type, IsReadOnly));

        foreach (BadObject o in base.InnerExecute(context))
        {
            yield return o;
        }
    }
}