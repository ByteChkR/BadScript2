using System.Text;

using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.VirtualMachine.Compiler;
namespace BadScript2.Parser.Expressions.Function;

/// <summary>
///     Implements the Function Expression
/// </summary>
public class BadFunctionExpression : BadExpression, IBadNamedExpression
{
    /// <summary>
    ///     The Function Body
    /// </summary>
    private readonly List<BadExpression> m_Body;


    /// <summary>
    ///     The Meta data of the Function
    /// </summary>
    private readonly BadMetaData? m_MetaData;

    /// <summary>
    ///     The Function parameters
    /// </summary>
    private readonly List<BadFunctionParameter> m_Parameters;

    /// <summary>
    ///     Constructor of the Function Expression
    /// </summary>
    /// <param name="name">The (optional) name of the function</param>
    /// <param name="parameter">The Function Parameters</param>
    /// <param name="block">The Function Body</param>
    /// <param name="position">Source Position of the Expression</param>
    /// <param name="isConstant">Indicates if this function can not be overwritten by another object</param>
    /// <param name="isStatic">Is the function Static?</param>
    /// <param name="compileLevel">Defines if the resulting function will be compiled</param>
    /// <param name="typeExpr">The (optional) Type Expression that is used to type-check the return value</param>
    /// <param name="metaData">The Meta data of the Function</param>
    /// <param name="isSingleLine">Is the function a single line function(e.g. a lambda expression)?</param>
    public BadFunctionExpression(
        BadWordToken? name,
        List<BadFunctionParameter> parameter,
        List<BadExpression> block,
        BadSourcePosition position,
        bool isConstant,
        BadMetaData? metaData,
        bool isSingleLine,
        bool isStatic,
        BadFunctionCompileLevel compileLevel = BadFunctionCompileLevel.None,
        BadExpression? typeExpr = null) :
        base(false, position)
    {
        Name = name;
        m_Parameters = parameter;
        m_Body = block;
        TypeExpression = typeExpr;
        IsConstantFunction = isConstant;
        m_MetaData = metaData;
        IsSingleLine = isSingleLine;
        IsStatic = isStatic;
        CompileLevel = compileLevel;
    }

    /// <summary>
    ///     Indicates if the function is a single line function(e.g. a lambda expression)?
    /// </summary>
    public bool IsSingleLine { get; }

    /// <summary>
    ///     Indicates if this function can not be overwritten by another object
    /// </summary>
    public bool IsConstantFunction { get; }

    /// <summary>
    ///     Indicates if the function is static
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    ///     The (optional) Type Expression that is used to type-check the return value
    /// </summary>
    public BadExpression? TypeExpression { get; }

    /// <summary>
    ///     The Function Parameters
    /// </summary>
    public IEnumerable<BadFunctionParameter> Parameters => m_Parameters;

    /// <summary>
    ///     The Function Body
    /// </summary>
    public IEnumerable<BadExpression> Body => m_Body;

    /// <summary>
    ///     The (optional) Function Name
    /// </summary>
    public BadWordToken? Name { get; private set; }

    /// <summary>
    ///     The Compile Level of the Function
    /// </summary>
    public BadFunctionCompileLevel CompileLevel { get; private set; }


    /// <inheritdoc cref="IBadNamedExpression.GetName" />
    public string? GetName()
    {
        return Name?.Text;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Sets the Compile Level of the Function
    /// </summary>
    /// <param name="level">The Compile Level</param>
    public void SetCompileLevel(BadFunctionCompileLevel level)
    {
        CompileLevel = level;
    }

    /// <summary>
    ///     Sets the Body of the Function
    /// </summary>
    /// <param name="body">The Body of the Function</param>
    public void SetBody(IEnumerable<BadExpression> body)
    {
        m_Body.Clear();
        m_Body.AddRange(body);
    }

    /// <inheritdoc cref="BadExpression.Optimize" />
    public override void Optimize()
    {
        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadConstantFoldingOptimizer.Optimize(m_Body[i]);
        }
    }

    /// <summary>
    ///     Returns the Header of the Function
    /// </summary>
    /// <returns>String Header of the Function</returns>
    public string GetHeader()
    {
        string level = CompileLevel switch
        {
            BadFunctionCompileLevel.Compiled => "compiled ",
            BadFunctionCompileLevel.CompiledFast => "compiled fast",
            _ => "",
        };

        return
            $"{level}{BadStaticKeys.FUNCTION_KEY} {Name?.ToString() ?? "<anonymous>"}({string.Join(", ", Parameters.Cast<object>())})";
    }


    /// <inheritdoc cref="BadExpression.ToString" />
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(GetHeader());
        sb.AppendLine();
        sb.AppendLine("{");

        foreach (BadExpression expression in Body)
        {
            sb.AppendLine($"\t{expression}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        if (TypeExpression != null)
        {
            foreach (BadExpression typeExpr in TypeExpression.GetDescendantsAndSelf())
            {
                yield return typeExpr;
            }
        }

        foreach (BadFunctionParameter parameter in m_Parameters)
        {
            if (parameter.TypeExpr == null)
            {
                continue;
            }

            foreach (BadExpression typeExpr in parameter.TypeExpr.GetDescendantsAndSelf())
            {
                yield return typeExpr;
            }
        }

        foreach (BadExpression descendant in m_Body.SelectMany(expression => expression.GetDescendantsAndSelf()))
        {
            yield return descendant;
        }
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

            obj = obj.Dereference();

            if (obj is not BadClassPrototype proto)
            {
                throw new BadRuntimeException(
                    $"Expected class prototype, but got {obj.GetType().Name}",
                    Position
                );
            }

            type = proto;
        }

        BadExpressionFunction f = new BadExpressionFunction(
            context.Scope,
            Name,
            m_Body,
            m_Parameters.Select(x => x.Initialize(context)).ToArray(),
            Position,
            IsConstantFunction,
            IsStatic,
            m_MetaData,
            type,
            IsSingleLine
        );

        BadFunction fFinal = CompileLevel switch
        {
            BadFunctionCompileLevel.Compiled => BadCompilerApi.CompileFunction(f, true),
            BadFunctionCompileLevel.CompiledFast => BadCompilerApi.CompileFunction(f, false),
            _ => f,
        };

        if (Name != null)
        {
            List<BadObject>? attributes = new List<BadObject>();
            foreach (BadObject? o in ComputeAttributes(context, attributes))
            {
                yield return o;
            }
            context.Scope.DefineVariable(
                Name.Text,
                fFinal,
                null,
                new BadPropertyInfo(fFinal.GetPrototype()),
                attributes.ToArray()
            );
        }
        else
        {
            if (Attributes.Any())
            {
                throw new BadRuntimeException(
                    "Anonymous functions cannot have attributes",
                    Position
                );
            }
        }

        yield return fFinal;
    }
}