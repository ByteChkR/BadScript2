using System;
using System.Linq;

using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler;

public class BadCompilerApi : BadInteropApi
{
    private readonly BadCompiler m_Compiler = new BadCompiler();
    public BadCompilerApi() : base("Compiler") { }

    public override void Load(BadTable target)
    {
        target.SetFunction<BadExpressionFunction, bool>("Compile", this.CompileFunction);
    }

    public BadObject CompileFunction(BadExpressionFunction func, bool useOverride)
    {
        return CompileFunction(m_Compiler, func, useOverride);
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