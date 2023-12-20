using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Functions;

/// <summary>
///     Interop Function taking an array of arguments
/// </summary>
public class BadInteropFunction : BadFunction
{
	private readonly Func<BadExecutionContext, BadObject[], BadObject> m_Func;

	public BadInteropFunction(
		BadWordToken? name,
		Func<BadObject[], BadObject> func,
		bool isStatic,
		BadClassPrototype returnType,
		params BadFunctionParameter[] parameters) : base(name, false, isStatic, returnType, parameters)
	{
		m_Func = (_, args) => func(args);
	}

	public BadInteropFunction(
		BadWordToken? name,
		Func<BadExecutionContext, BadObject[], BadObject> func,
		bool isStatic,
		BadClassPrototype returnType,
		params BadFunctionParameter[] parameters) : base(name, false, isStatic, returnType, parameters)
	{
		m_Func = func;
	}


	public static BadInteropFunction Create(
		Func<BadObject[], BadObject> func,
		bool isStatic,
		BadClassPrototype returnType,
		params string[] names)
	{
		BadInteropFunction function = new BadInteropFunction(null,
			func,
			isStatic,
			returnType,
			names.Select(x => (BadFunctionParameter)x).ToArray());

		return function;
	}


	protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
	{
		CheckParameters(args);

		yield return m_Func.Invoke(caller, args);
	}
}
