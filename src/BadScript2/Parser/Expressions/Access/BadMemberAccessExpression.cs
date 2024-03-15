using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Member Access to set or get properties from an object.
///     LEFT.RIGHT
/// </summary>
public class BadMemberAccessExpression : BadExpression, IBadAccessExpression
{
	private readonly List<BadExpression> m_GenericArguments;
	/// <summary>
	///     Constructor for the Member Access Expression.
	/// </summary>
	/// <param name="left">Left side of the expression</param>
	/// <param name="right">Right side of the expression</param>
	/// <param name="position">Position inside the source code</param>
	/// <param name="nullChecked">
	///     Null-check the result of the left side of the expression. If null. this expression returns
	///     BadObject.Null
	/// </param>
	public BadMemberAccessExpression(
        BadExpression left,
        BadWordToken right,
        BadSourcePosition position,
        List<BadExpression> genericArguments,
        bool nullChecked = false) : base(
        false,
        position
    )
    {
        Left = left;
        Right = right;
        m_GenericArguments = genericArguments;
        NullChecked = nullChecked;
    }

	/// <summary>
	///     Left side of the expression
	/// </summary>
	public BadExpression Left { get; private set; }

	/// <summary>
	///     Right Side of the expression
	/// </summary>
	public BadWordToken Right { get; }

	/// <summary>
	///     Property that indicates if the result of the left side of the expression should be null-checked.
	/// </summary>
	public bool NullChecked { get; }

    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Left.GetDescendantsAndSelf();
    }

    public override void Optimize()
    {
        Left = BadConstantFoldingOptimizer.Optimize(Left);
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        if (NullChecked && !left.HasProperty(Right.Text, context.Scope))
        {
            yield return BadObject.Null;
        }
        else
        {
            BadObject ret = left.GetProperty(Right.Text, context.Scope);

            if (m_GenericArguments.Count != 0)
            {
	            ret = ret.Dereference();
	            if (ret is not IBadGenericObject genType)
	            {
		            throw BadRuntimeException.Create(
			            context.Scope,
			            $"Object {ret} does not support generic arguments",
			            Position
		            );
	            }
	            BadObject[] genParams = new BadObject[m_GenericArguments.Count];
	            for (int i = 0; i < m_GenericArguments.Count; i++)
	            {
		            foreach (var o in m_GenericArguments[i].Execute(context))
		            {
			            genParams[i] = o;
		            }

		            genParams[i] = genParams[i].Dereference();
	            }

	            yield return genType.CreateGeneric(genParams);
            }
			else{
				yield return ret;
			}	
        }
    }


    public override string ToString()
    {
        return $"({Left}{(NullChecked ? "?" : "")}.{Right})";
    }
}