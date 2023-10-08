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
public class BadFunctionExpression : BadExpression
{
    /// <summary>
    ///     The Function Body
    /// </summary>
    private readonly List<BadExpression> m_Body;


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
    /// <param name="compileLevel">Defines if the resulting function will be compiled</param>
    /// <param name="typeExpr">The (optional) Type Expression that is used to type-check the return value</param>
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

    public bool IsSingleLine { get; }

    /// <summary>
    ///     Indicates if this function can not be overwritten by another object
    /// </summary>
    public bool IsConstantFunction { get; }

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
    public BadWordToken? Name { get; }

    public BadFunctionCompileLevel CompileLevel { get; }

    public void SetBody(IEnumerable<BadExpression> body)
    {
        m_Body.Clear();
        m_Body.AddRange(body);
    }

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
        return
            $"{BadStaticKeys.FunctionKey} {Name?.ToString() ?? "<anonymous>"}({string.Join(", ", Parameters.Cast<object>())})";
    }


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
            if (parameter.TypeExpr != null)
            {
                foreach (BadExpression typeExpr in parameter.TypeExpr.GetDescendantsAndSelf())
                {
                    yield return typeExpr;
                }
            }
        }

        foreach (BadExpression expression in m_Body)
        {
            foreach (BadExpression descendant in expression.GetDescendantsAndSelf())
            {
                yield return descendant;
            }
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        if (TypeExpression != null)
        {
            BadObject obj = BadObject.Null;

            foreach (BadObject o in TypeExpression.Execute(context))
            {
                obj = o;
            }

            obj = obj.Dereference();

            if (obj is not BadClassPrototype proto)
            {
                throw new BadRuntimeException(
                    $"Expected class prototype, but got {obj.GetType().Name}",
                    Position
                );
            }
        }

        BadExpressionFunction f = new BadExpressionFunction(
            context.Scope,
            Name,
            m_Body,
            m_Parameters.Select(x => x.Initialize(context)).ToArray(),
            Position,
            IsConstantFunction,
            IsStatic,
            m_MetaData
        );

        BadFunction fFinal = f;

        if (CompileLevel == BadFunctionCompileLevel.Compiled)
        {
            fFinal = BadCompilerApi.CompileFunction(BadCompiler.Instance, f, true);
        }
        else if (CompileLevel == BadFunctionCompileLevel.CompiledFast)
        {
            fFinal = BadCompilerApi.CompileFunction(BadCompiler.Instance, f, false);
        }


        if (Name != null)
        {
            context.Scope.DefineVariable(BadObject.Wrap(Name.Text), fFinal);
        }

        yield return fFinal;
    }
}