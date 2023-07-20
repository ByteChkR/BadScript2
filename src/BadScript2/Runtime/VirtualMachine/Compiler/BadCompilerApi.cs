using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

public class BadCompilerApi : BadInteropApi
{
	public BadCompilerApi() : base("Compiler") { }

	protected override void LoadApi(BadTable target)
	{
		target.SetFunction<BadExpressionFunction, bool>("Compile", CompileFunction);
	}

	public BadObject CompileFunction(BadExpressionFunction func, bool useOverride)
	{
		return CompileFunction(BadCompiler.Instance, func, useOverride);
	}

	public static BadCompiledFunction CompileFunction(
		BadCompiler compiler,
		BadExpressionFunction func,
		bool useOverride)
	{
		BadInstruction[] instrs = compiler.Compile(func.Body).ToArray();
		

		return new BadCompiledFunction(instrs,
			useOverride,
			func.ParentScope,
			func.Position,
			func.Name,
			func.IsConstant,
			func.IsStatic,
			func.MetaData,
			func.Parameters);
	}
}
