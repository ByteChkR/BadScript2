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
        target.SetFunction<BadExpressionFunction, bool>("Compile", this.CompileFunction);
    }

    public BadObject CompileFunction(BadExpressionFunction func, bool useOverride)
    {
        return CompileFunction(BadCompiler.Instance, func, useOverride);
    }

    public static BadCompiledFunction CompileFunction(BadCompiler compiler, BadExpressionFunction func, bool useOverride)
    {
        BadInstruction[] instrs = compiler.Compile(func.Body).ToArray();
        //int current = 0;
        // foreach (BadInstruction instruction in instrs)
        // {
        //     Console.WriteLine($"{current++}\t: {instruction}");
        // }

        return new BadCompiledFunction(instrs, useOverride, func.ParentScope, func.Position, func.Name, func.IsConstant, func.Parameters);
    }
}