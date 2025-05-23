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
    public BadVariableDefinitionExpression(string name,
                                           BadSourcePosition position,
                                           BadExpression? typeExpression = null,
                                           bool isReadOnly = false) : base(name,
                                                                           position,
                                                                           Array.Empty<BadExpression>()
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
        return IsReadOnly
                   ? $"{BadStaticKeys.CONSTANT_DEFINITION_KEY} {Name}"
                   : $"{BadStaticKeys.VARIABLE_DEFINITION_KEY} {Name}";
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadClassPrototype type = BadAnyPrototype.Instance;

        if (TypeExpression != null)
        {
            BadObject obj = BadObject.Null;

            foreach (BadObject o in TypeExpression.Execute(context))
            {
                obj = o;

                yield return o;
            }

            obj = obj.Dereference(Position);

            if (obj is not BadClassPrototype proto)
            {
                throw BadRuntimeException.Create(context.Scope, "Type expression must be a class prototype", Position);
            }

            type = proto;
        }

        if (type == BadVoidPrototype.Instance)
        {
            throw BadRuntimeException.Create(context.Scope, "Cannot declare a variable of type 'void'", Position);
        }

        if (context.Scope.HasLocal(Name, context.Scope, false))
        {
            throw BadRuntimeException.Create(context.Scope, $"Variable '{Name}' is already defined", Position);
        }

        List<BadObject> attributes = new List<BadObject>();

        foreach (BadObject? o in ComputeAttributes(context, attributes))
        {
            yield return o;
        }

        context.Scope.DefineVariable(Name,
                                     BadObject.Null,
                                     null,
                                     new BadPropertyInfo(type, IsReadOnly),
                                     attributes.ToArray()
                                    );

        foreach (BadObject o in base.InnerExecute(context))
        {
            yield return o;
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        if (TypeExpression != null)
        {
            foreach (BadExpression typeExpression in TypeExpression.GetDescendantsAndSelf())
            {
                yield return typeExpression;
            }
        }

        foreach (BadExpression descendant in base.GetDescendants())
        {
            yield return descendant;
        }
    }
}