using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.VirtualMachine;

public class BadCompiledFunction : BadFunction
{
	private readonly BadInstruction[] m_Instructions;
	private readonly BadScope m_ParentScope;
	private readonly BadSourcePosition m_Position;
	private readonly bool m_UseOverrides;

	public BadCompiledFunction(
		BadInstruction[] instructions,
		bool useOverrides,
		BadScope parentScope,
		BadSourcePosition position,
		BadWordToken? name,
		bool isConstant,
		bool isStatic,
		BadMetaData? metaData,
		params BadFunctionParameter[] parameters) : base(name,
		isConstant,
		isStatic,
		parameters)
	{
		m_Instructions = instructions;
		m_UseOverrides = useOverrides;
		m_ParentScope = parentScope;
		m_Position = position;
		MetaData = metaData ?? BadMetaData.Empty;
	}

	public override BadMetaData MetaData { get; }

	protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
	{
		BadExecutionContext ctx = new BadExecutionContext(m_ParentScope.CreateChild("Compiled Function",
			caller.Scope,
			null,
			BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow));
		ApplyParameters(ctx, args, m_Position);
		BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(m_Instructions, m_UseOverrides);

		foreach (BadObject o in vm.Execute(ctx))
		{
			yield return o;
		}

		if (ctx.Scope.ReturnValue != null)
		{
			yield return ctx.Scope.ReturnValue;
		}
		else
		{
			yield return Null;
		}

		if (ctx.Scope.Error != null)
		{
			caller.Scope.SetErrorObject(ctx.Scope.Error);
		}
	}

	public override string ToString()
	{
		string str = base.ToString() + " at " + m_Position.GetPositionInfo() + '\n';

		for (int i = 0; i < m_Instructions.Length; i++)
		{
			str += i + ":\t" + m_Instructions[i] + '\n';
		}

		return str;
	}
}
