using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

/// <summary>
/// Contains the Type Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Types;

/// <summary>
///     Implements the Class Prototype Expression
/// </summary>
public class BadClassPrototypeExpression : BadExpression, IBadNamedExpression
{
    /// <summary>
    ///     The Base Classes
    /// </summary>
    private readonly BadExpression[] m_BaseClasses;

    /// <summary>
    ///     The Class Body
    /// </summary>
    private readonly List<BadExpression> m_Body;

    private readonly BadWordToken[] m_GenericParameters;

    /// <summary>
    ///     The Meta data of the Class
    /// </summary>
    private readonly BadMetaData? m_MetaData;

    /// <summary>
    ///     The Static Class Body
    /// </summary>
    private readonly List<BadExpression> m_StaticBody;

    /// <summary>
    ///     Constructor of the Class Prototype Expression
    /// </summary>
    /// <param name="name">The Class name</param>
    /// <param name="body">The Class Body</param>
    /// <param name="staticBody">The Static Class Body</param>
    /// <param name="baseClasses">The (optional) base class</param>
    /// <param name="position">The Source Position of the Expression</param>
    /// <param name="metaData">The metadata of the Class</param>
    /// <param name="genericParameters">The Generic Parameters of this Type</param>
    public BadClassPrototypeExpression(string name,
                                       IEnumerable<BadExpression> body,
                                       IEnumerable<BadExpression> staticBody,
                                       BadExpression[] baseClasses,
                                       BadSourcePosition position,
                                       BadMetaData? metaData,
                                       BadWordToken[] genericParameters) : base(false, position)
    {
        Name = name;
        m_Body = new List<BadExpression>(body);
        m_BaseClasses = baseClasses;
        m_MetaData = metaData;
        m_GenericParameters = genericParameters;
        m_StaticBody = new List<BadExpression>(staticBody);
    }

    /// <summary>
    ///     The Class Body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;

    /// <summary>
    ///     The Static Class Body
    /// </summary>
    public IEnumerable<BadExpression> StaticBody => m_StaticBody;

    /// <summary>
    ///     The Class Name
    /// </summary>
    public string Name { get; }

#region IBadNamedExpression Members

    /// <inheritdoc cref="IBadNamedExpression.GetName" />
    public string? GetName()
    {
        return Name;
    }

#endregion

    /// <summary>
    ///     Sets the body of the class
    /// </summary>
    /// <param name="body">The body</param>
    public void SetBody(IEnumerable<BadExpression> body)
    {
        m_Body.Clear();
        m_Body.AddRange(body);
    }

    /// <summary>
    ///     Sets the static body of the class
    /// </summary>
    /// <param name="body">The static body</param>
    public void SetStaticBody(IEnumerable<BadExpression> body)
    {
        m_StaticBody.Clear();
        m_StaticBody.AddRange(body);
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadConstantFoldingOptimizer.Optimize(m_Body[i]);
        }

        for (int i = 0; i < m_StaticBody.Count; i++)
        {
            m_StaticBody[i] = BadConstantFoldingOptimizer.Optimize(m_StaticBody[i]);
        }
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        foreach (BadExpression baseClass in m_BaseClasses)
        {
            foreach (BadExpression baseExpr in baseClass.GetDescendantsAndSelf())
            {
                yield return baseExpr;
            }
        }

        foreach (BadExpression e in m_Body.SelectMany(expression => expression.GetDescendantsAndSelf()))
        {
            yield return e;
        }

        foreach (BadExpression e in m_StaticBody.SelectMany(expression => expression.GetDescendantsAndSelf()))
        {
            yield return e;
        }
    }

    /// <summary>
    ///     Returns the Base Class prototype and the implemented interfaces
    /// </summary>
    /// <param name="context">The Current Execution Context</param>
    /// <param name="interfaces">The implemented interfaces</param>
    /// <returns>The Base Class Prototype</returns>
    /// <exception cref="BadRuntimeException">If the Base Class Expression returns an invalid Object</exception>
    private BadClassPrototype GetPrototype(BadExecutionContext context,
                                           out BadInterfacePrototype[] interfaces)
    {
        BadClassPrototype baseClass = BadAnyPrototype.Instance;

        List<BadInterfacePrototype> interfacesList = new List<BadInterfacePrototype>();

        for (int i = 0; i < m_BaseClasses.Length; i++)
        {
            BadExpression baseClassExpr = m_BaseClasses[i];

            BadObject[] baseClassObj = context.Execute(baseClassExpr)
                                              .ToArray();

            if (baseClassObj.Length != 1)
            {
                throw new
                    BadRuntimeException($"Base Class Expression {baseClassExpr} returned {baseClassObj.Length} Objects. Expected 1.",
                                        baseClassExpr.Position
                                       );
            }

            BadObject o = baseClassObj[0]
                .Dereference();

            switch (o)
            {
                case BadInterfacePrototype iface:
                    interfacesList.Add(iface);

                    break;
                case BadClassPrototype when i != 0:
                    throw new
                        BadRuntimeException($"Base Class Expression {baseClassExpr} returned a Class Prototype. Expected an Interface Prototype.",
                                            baseClassExpr.Position
                                           );
                case BadClassPrototype p:
                    baseClass = p;

                    if (baseClass == BadVoidPrototype.Instance)
                    {
                        throw BadRuntimeException.Create(context.Scope, "Base Class cannot be 'void'.", Position);
                    }

                    break;
                default:
                    throw new
                        BadRuntimeException($"Base Class Expression {baseClassExpr} returned an Object of Type {o}. Expected a Class Prototype or an Interface Prototype.",
                                            baseClassExpr.Position
                                           );
            }
        }

        interfaces = interfacesList.ToArray();

        return baseClass;
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadExecutionContext MakeGenericContext(BadObject[] typeArgs)
        {
            if (typeArgs.Length != m_GenericParameters.Length)
            {
                throw new BadRuntimeException("Invalid Type Argument Count");
            }

            BadExecutionContext genericContext =
                new BadExecutionContext(context.Scope.CreateChild("GenericContext", context.Scope, null));

            for (int i = 0; i < m_GenericParameters.Length; i++)
            {
                BadWordToken genericParameter = m_GenericParameters[i];

                if (typeArgs[i] == BadVoidPrototype.Instance)
                {
                    throw BadRuntimeException.Create(context.Scope,
                                                     "Cannot use 'void' as generic type parameter",
                                                     Position
                                                    );
                }

                genericContext.Scope.DefineVariable(genericParameter.Text,
                                                    typeArgs[i],
                                                    genericContext.Scope,
                                                    new BadPropertyInfo(BadClassPrototype.Prototype, true)
                                                   );
            }

            return genericContext;
        }

        BadClassPrototype GetBaseClass(BadObject[] typeArgs)
        {
            BadExecutionContext ctx = MakeGenericContext(typeArgs);

            return GetPrototype(ctx, out _);
        }

        BadScope GetStaticScope(BadObject[] typeArgs)
        {
            BadExecutionContext ctx = MakeGenericContext(typeArgs);

            BadExecutionContext staticContext =
                new BadExecutionContext(ctx.Scope.CreateChild($"static:{Name}", ctx.Scope, true));

            if (m_StaticBody.Count != 0)
            {
                foreach (BadObject _ in staticContext.Execute(m_StaticBody)) { }
            }

            return staticContext.Scope;
        }

        BadInterfacePrototype[] GetInterfaces(BadObject[] typeArgs)
        {
            BadExecutionContext ctx = MakeGenericContext(typeArgs);
            GetPrototype(ctx, out BadInterfacePrototype[] interfaces);

            return interfaces;
        }

        BadClassPrototype p = new BadExpressionClassPrototype(Name,
                                                              context.Scope,
                                                              m_Body.ToArray(),
                                                              GetBaseClass,
                                                              GetInterfaces,
                                                              m_MetaData,
                                                              GetStaticScope,
                                                              m_GenericParameters.Select(x => x.Text)
                                                                  .ToArray()
                                                             );
        context.Scope.DefineVariable(Name, p);

        yield return p;
    }
}