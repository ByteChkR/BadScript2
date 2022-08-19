using System.Text;

using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Parser.Expressions.Function;

public class BadFunctionExpression : BadExpression
{
    private readonly List<BadExpression> m_Body;
    private readonly List<BadFunctionParameter> m_Parameters;
    private readonly BadExpression? m_TypeExpr;
    private readonly bool IsConstantFunction;

    public BadFunctionExpression(
        BadWordToken? name,
        List<BadFunctionParameter> parameter,
        List<BadExpression> block,
        BadSourcePosition position,
        bool isConstant,
        BadExpression? typeExpr = null) :
        base(false, position)
    {
        Name = name;
        m_Parameters = parameter;
        m_Body = block;
        m_TypeExpr = typeExpr;
        IsConstantFunction = isConstant;
    }

    private IEnumerable<BadFunctionParameter> Parameters => m_Parameters;
    private IEnumerable<BadExpression> Body => m_Body;
    private BadWordToken? Name { get; }

    public override void Optimize()
    {
        for (int i = 0; i < m_Body.Count; i++)
        {
            m_Body[i] = BadExpressionOptimizer.Optimize(m_Body[i]);
        }
    }

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

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        if (m_TypeExpr != null)
        {
            BadObject obj = BadObject.Null;
            foreach (BadObject o in m_TypeExpr.Execute(context))
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
            IsConstantFunction
        );

        if (Name != null)
        {
            context.Scope.DefineVariable(BadObject.Wrap(Name.Text), f);
        }

        yield return f;
    }
}