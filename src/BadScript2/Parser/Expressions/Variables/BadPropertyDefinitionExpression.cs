using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Variables;

/// <summary>
/// Implements a Property Definition Expression
/// </summary>
public class BadPropertyDefinitionExpression : BadExpression
{
    /// <summary>
    /// Constructor of the Property Definition Expression
    /// </summary>
    /// <param name="name">The Name of the Property</param>
    /// <param name="position">The Source Position of the Expression</param>
    /// <param name="getExpression">The Get Expression</param>
    /// <param name="typeExpression">The (optional) Type of the Property</param>
    /// <param name="setExpression">The optional Set Expression</param>
    /// <param name="isReadOnly">Indicates if the Property will be declared as Read-Only</param>
    public BadPropertyDefinitionExpression(BadWordToken name,
        BadSourcePosition position,
        BadExpression getExpression,
        BadExpression? typeExpression = null,
        BadExpression? setExpression = null,
        bool isReadOnly = false) : base(false, position)
    {
        Name = name;
        IsReadOnly = isReadOnly;
        TypeExpression = typeExpression;
        GetExpression = getExpression;
        SetExpression = setExpression;
    }

    /// <summary>
    /// The Name of the Property
    /// </summary>
    public BadWordToken Name { get; }

    /// <summary>
    /// Indicates if the Property will be declared as Read-Only
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// The (optional) Type of the Property
    /// </summary>
    public BadExpression? TypeExpression { get; }

    /// <summary>
    /// The Get Expression
    /// </summary>
    public BadExpression GetExpression { get; }

    /// <summary>
    /// The optional Set Expression
    /// </summary>
    public BadExpression? SetExpression { get; }

    /// <inheritdoc />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        yield return GetExpression;

        if (TypeExpression != null)
        {
            yield return TypeExpression;
        }

        if (SetExpression != null)
        {
            yield return SetExpression;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        if (context.Scope.ClassObject == null)
        {
            throw BadRuntimeException.Create(context.Scope, "Can only define properties in class scope", Position);
        }

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
                throw new BadRuntimeException("Type expression must be a class prototype", Position);
            }

            type = proto;
        }

        List<BadObject> attributes = new List<BadObject>();

        foreach (BadObject? o in ComputeAttributes(context, attributes))
        {
            yield return o;
        }

        context.Scope.DefineProperty(Name.Text, type, GetExpression, SetExpression, context, attributes.ToArray());
    }
}