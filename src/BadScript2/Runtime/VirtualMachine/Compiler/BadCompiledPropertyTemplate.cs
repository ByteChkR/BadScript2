using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Structured representation of a property definition used by compiled class templates.
/// </summary>
public sealed class BadCompiledPropertyTemplate
{
    public BadCompiledPropertyTemplate(BadPropertyDefinitionExpression expression)
    {
        BadFunctionCompileLevel getCompileLevel = expression.GetCompileLevel;
        BadFunctionCompileLevel setCompileLevel = expression.SetCompileLevel;

        Name = expression.Name.Text;
        TypeExpression = expression.TypeExpression;
        Getter = new BadCompiledPropertyAccessorTemplate(expression.GetExpression,
                                                         getCompileLevel != BadFunctionCompileLevel.None,
                                                         getCompileLevel != BadFunctionCompileLevel.CompiledFast
                                                        );
        Setter = expression.SetExpression != null
                     ? new BadCompiledPropertyAccessorTemplate(expression.SetExpression,
                                                               setCompileLevel != BadFunctionCompileLevel.None,
                                                               setCompileLevel != BadFunctionCompileLevel.CompiledFast
                                                              )
                     : null;
        AttributeExpressions = expression.Attributes.ToArray();
        IsReadOnly = expression.IsReadOnly;
    }

    /// <summary>
    /// Property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Optional type expression.
    /// </summary>
    public BadExpression? TypeExpression { get; }

    /// <summary>
    /// Getter accessor template.
    /// </summary>
    public BadCompiledPropertyAccessorTemplate Getter { get; }

    /// <summary>
    /// Optional setter accessor template.
    /// </summary>
    public BadCompiledPropertyAccessorTemplate? Setter { get; }

    /// <summary>
    /// Original property attribute expressions.
    /// </summary>
    public IReadOnlyList<BadExpression> AttributeExpressions { get; }

    /// <summary>
    /// Indicates whether the original property was declared read-only.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Materializes the property definition in the given execution context.
    /// </summary>
    public IEnumerable<BadObject> Define(BadExecutionContext context, bool requireClassScope = true)
    {
        if (requireClassScope && context.Scope.ClassObject == null)
        {
            throw BadRuntimeException.Create(context.Scope, "Can only define properties in class scope");
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

            obj = obj.Dereference(TypeExpression.Position);

            if (obj is not BadClassPrototype proto)
            {
                throw new BadRuntimeException("Type expression must be a class prototype", TypeExpression.Position);
            }

            type = proto;
        }

        List<BadObject> attributes = new List<BadObject>();

        if (AttributeExpressions.Count != 0)
        {
            PropertyDefinitionProxy proxy = new PropertyDefinitionProxy(Name, Getter.Expression.Position, AttributeExpressions);

            foreach (BadObject o in proxy.EvaluateAttributes(context, attributes))
            {
                yield return o;
            }
        }

        context.Scope.DefineProperty(Name, type, Getter, Setter, context, attributes.ToArray());
    }

    private sealed class PropertyDefinitionProxy : BadExpression
    {
        public PropertyDefinitionProxy(string name, BadSourcePosition position, IEnumerable<BadExpression> attributes) : base(false, position)
        {
            Name = name;
            SetAttributes(attributes);
        }

        public string Name { get; }

        public override IEnumerable<BadExpression> GetDescendants()
        {
            foreach (BadExpression attribute in Attributes)
            {
                foreach (BadExpression descendant in attribute.GetDescendantsAndSelf())
                {
                    yield return descendant;
                }
            }
        }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            yield break;
        }
    }
}


