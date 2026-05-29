using BadScript2.Common;
using BadScript2.Debugging;
using BadScript2.Interop.Common;
using BadScript2.Runtime.Error;
using BadScript2.Parser.Expressions;
using BadScript2.Parser;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.Settings;
using BadScript2.Runtime.VirtualMachine.Compiler;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

namespace BadScript2.Tests;

public class BadVmCompilerTests
{
    private static readonly BadSourcePosition s_Position = BadSourcePosition.FromSource("test", 0, 4);

    private static BadExpressionClassPrototype CreateAttributePrototype(BadScope scope, string name)
    {
        return new BadExpressionClassPrototype(name,
                                               scope,
                                               [],
                                               [],
                                               _ => BadAnyPrototype.Instance,
                                               _ => [BadNativeClassBuilder.Attribute],
                                               null,
                                               _ => scope.CreateChild($"static:{name}", scope, true),
                                               Array.Empty<string>()
                                              );
    }

    [Test]
    public void PropertyDefinitionExpression_CompilesToDefinePropertyInstruction()
    {
        BadPropertyDefinitionExpression expression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadNumberExpression(1, s_Position)
                                               );
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(expression);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions, Has.Length.EqualTo(1));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.DefineProperty));
        Assert.That(instructions[0].Arguments[0], Is.EqualTo("Value"));
    }

    [Test]
    public void VariableDefinitionExpression_WithAttributes_CompilesToDefVarInstruction()
    {
        BadVariableDefinitionExpression expression =
            new BadVariableDefinitionExpression("Value", s_Position);
        expression.SetAttributes([new BadVariableExpression("SimpleAttribute", s_Position)]);
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(expression);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions, Has.Length.EqualTo(1));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.DefVar));
        Assert.That(instructions[0].Arguments[2], Is.TypeOf<BadExpression[]>());
        Assert.That(((BadExpression[])instructions[0].Arguments[2]), Has.Length.EqualTo(1));
    }

    [Test]
    public void VariableDefinitionExpression_WithAttributes_RuntimeDefinesAttributes()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadExpressionClassPrototype attributePrototype = CreateAttributePrototype(context.Scope, "SimpleAttribute");
        context.Scope.DefineVariable("SimpleAttribute",
                                     attributePrototype,
                                     context.Scope,
                                     new BadPropertyInfo(BadClassPrototype.Prototype, true)
                                    );

        BadVariableDefinitionExpression expression =
            new BadVariableDefinitionExpression("Value", s_Position);
        expression.SetAttributes([new BadVariableExpression("SimpleAttribute", s_Position)]);
        BadInstruction[] instructions = BadCompiler.Compile([expression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        Assert.That(context.Scope.Attributes.ContainsKey("Value"), Is.True);
        Assert.That(context.Scope.Attributes["Value"], Has.Length.EqualTo(1));
        Assert.That(context.Scope.Attributes["Value"][0], Is.TypeOf<BadClass>());
        Assert.That(((BadClass)context.Scope.Attributes["Value"][0]).Prototype, Is.SameAs(attributePrototype));
    }

    [Test]
    public void CompileExpressionSequence_WithClearStack_EmitsClearStackAfterEachExpression()
    {
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile([new BadNumberExpression(1, s_Position), new BadNumberExpression(2, s_Position)]);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions.Select(x => x.OpCode),
                    Is.EqualTo(new[]
                    {
                        BadOpCode.Push,
                        BadOpCode.ClearStack,
                        BadOpCode.Push,
                        BadOpCode.ClearStack
                    })
                   );
    }

    [Test]
    public void FunctionExpression_CompilesToCreateFunctionInstruction()
    {
        BadFunctionExpression expression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(1, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(expression);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions, Has.Length.EqualTo(1));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.CreateFunction));
    }

    [Test]
    public void InvocationExpression_WithCompiledFunctionTarget_CompilesToInvokeCompiled()
    {
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(1, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadInvocationExpression invocation = new BadInvocationExpression(functionExpression, [], s_Position);
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(invocation);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions.Select(x => x.OpCode),
                    Is.EqualTo(new[]
                    {
                        BadOpCode.CreateFunction,
                        BadOpCode.InvokeCompiled,
                    })
                   );
    }

    [Test]
    public void CompiledFunctionTemplate_BuildsSymbolTableForParametersAndLocals()
    {
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      ["input"],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("local", s_Position),
                                                                  new BadNumberExpression(1, s_Position),
                                                                  s_Position),
                                          new BadReturnExpression(new BadVariableExpression("local", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(functionExpression);
        BadInstruction[] instructions = context.GetInstructions();
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)instructions[0].Arguments[0];

        Assert.That(template.SymbolTable, Is.Not.Null);
        Assert.That(template.SymbolTable!.ParameterCount, Is.EqualTo(1));
        Assert.That(template.SymbolTable.LocalCount, Is.EqualTo(1));
        Assert.That(template.SymbolTable.GetSymbol("input").SlotIndex, Is.EqualTo(0));
        Assert.That(template.SymbolTable.GetSymbol("local").SlotIndex, Is.EqualTo(1));
    }

    [Test]
    public void CompiledFunctionTemplate_RuntimeUsesSlotBackedParametersAndLocals()
    {
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      ["input"],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("local", s_Position),
                                                                  new BadVariableExpression("input", s_Position),
                                                                  s_Position),
                                          new BadReturnExpression(new BadVariableExpression("local", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);

        compileContext.Compile(functionExpression);
        BadInstruction[] instructions = compileContext.GetInstructions();
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)instructions[0].Arguments[0];
        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadObject created = BadObject.Null;
        foreach (BadObject o in template.Instantiate(definitionContext))
        {
            created = o;
        }

        BadCompiledFunction compiled = (BadCompiledFunction)created;

        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject result = BadObject.Null;
        foreach (BadObject o in compiled.Invoke([(BadObject)new BadNumber(77)], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(77));
    }

    [Test]
    public void CompiledFunctionTemplate_ClosureReadsOuterVariableCorrectly()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [new BadReturnExpression(new BadVariableExpression("x", s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("x", s_Position),
                                                                  new BadNumberExpression(10, s_Position),
                                                                  s_Position),
                                          innerFunction,
                                          new BadReturnExpression(new BadVariableExpression("Inner", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject outerCreated = BadObject.Null;
        foreach (BadObject o in outerTemplate.Instantiate(definitionContext))
        {
            outerCreated = o;
        }

        BadCompiledFunction outerCompiled = (BadCompiledFunction)outerCreated;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject innerCreated = BadObject.Null;
        foreach (BadObject o in outerCompiled.Invoke([], caller))
        {
            innerCreated = o;
        }

        BadCompiledFunction innerCompiled = (BadCompiledFunction)innerCreated;
        BadObject result = BadObject.Null;
        foreach (BadObject o in innerCompiled.Invoke([], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(10));
    }

    [Test]
    public void CompiledFunctionTemplate_WithNestedFunction_BuildsSymbolTableForCaptureAwarePath()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(1, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [innerFunction, new BadReturnExpression(new BadNumberExpression(0, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);

        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        Assert.That(template.SymbolTable, Is.Not.Null);
        Assert.That(template.SymbolTable!.CaptureCount, Is.EqualTo(0));
    }

    [Test]
    public void CompiledFunctionTemplate_SlotBoundParameters_AreNotMaterializedIntoScopeBeforeExecution()
    {
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      ["input"],
                                      [new BadReturnExpression(new BadVariableExpression("input", s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(functionExpression);
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];
        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadObject created = BadObject.Null;
        foreach (BadObject o in template.Instantiate(definitionContext))
        {
            created = o;
        }

        BadCompiledFunction compiled = (BadCompiledFunction)created;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        using BadExecutionContext invocationContext = compiled.CreateExecutionContext(caller, [(BadObject)new BadNumber(42)]);

        Assert.That(invocationContext.Scope.HasLocal("input", invocationContext.Scope, false), Is.False);
    }

    [Test]
    public void CompiledFunctionTemplate_SlotBackedReadOnlyLocal_StillThrowsOnReassignment()
    {
        BadVariableDefinitionExpression readOnlyLocal =
            new BadVariableDefinitionExpression("local", s_Position, null, true);
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      [],
                                      [
                                          new BadAssignExpression(readOnlyLocal, new BadNumberExpression(1, s_Position), s_Position),
                                          new BadAssignExpression(new BadVariableExpression("local", s_Position), new BadNumberExpression(2, s_Position), s_Position),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(functionExpression);
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];
        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadObject created = BadObject.Null;
        foreach (BadObject o in template.Instantiate(definitionContext))
        {
            created = o;
        }

        BadCompiledFunction compiled = (BadCompiledFunction)created;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());

        Assert.Throws<BadRuntimeException>(() =>
        {
            foreach (BadObject _ in compiled.Invoke([], caller))
            {
            }
        });
    }

    [Test]
    public void CompiledFunctionTemplate_SlotBackedTypedLocal_StillValidatesAssignments()
    {
        BadVariableDefinitionExpression typedLocal =
            new BadVariableDefinitionExpression("local",
                                                s_Position,
                                                new BadConstantExpression(s_Position, BadNativeClassBuilder.GetNative("num")),
                                                false);
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      [],
                                      [
                                          new BadAssignExpression(typedLocal, new BadNumberExpression(1, s_Position), s_Position),
                                          new BadAssignExpression(new BadVariableExpression("local", s_Position), new BadStringExpression("\"x\"", s_Position), s_Position),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(functionExpression);
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];
        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadObject created = BadObject.Null;
        foreach (BadObject o in template.Instantiate(definitionContext))
        {
            created = o;
        }

        BadCompiledFunction compiled = (BadCompiledFunction)created;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());

        Assert.Throws<BadRuntimeException>(() =>
        {
            foreach (BadObject _ in compiled.Invoke([], caller))
            {
            }
        });
    }

    [Test]
    public void CompiledFunctionTemplate_WithDisabledSlotLocalFastPath_DoesNotBuildSymbolTable()
    {
        bool oldValue = BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath;
        BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath = false;

        try
        {
            BadFunctionExpression functionExpression =
                new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                          ["input"],
                                          [new BadReturnExpression(new BadVariableExpression("input", s_Position), s_Position, false)],
                                          s_Position,
                                          false,
                                          null,
                                          false,
                                          false,
                                          BadFunctionCompileLevel.Compiled
                                         );
            BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);

            compileContext.Compile(functionExpression);
            BadCompiledFunctionTemplate template =
                (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

            Assert.That(template.SymbolTable, Is.Null);
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath = oldValue;
        }
    }

    [Test]
    public void CompiledFunctionTemplate_WithDisabledSlotLocalFastPath_MaterializesParametersIntoScope()
    {
        bool oldValue = BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath;
        BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath = false;

        try
        {
            BadFunctionExpression functionExpression =
                new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                          ["input"],
                                          [new BadReturnExpression(new BadVariableExpression("input", s_Position), s_Position, false)],
                                          s_Position,
                                          false,
                                          null,
                                          false,
                                          false,
                                          BadFunctionCompileLevel.Compiled
                                         );
            BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
            compileContext.Compile(functionExpression);
            BadCompiledFunctionTemplate template =
                (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];
            using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());

            BadObject created = BadObject.Null;
            foreach (BadObject o in template.Instantiate(definitionContext))
            {
                created = o;
            }

            BadCompiledFunction compiled = (BadCompiledFunction)created;
            using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
            using BadExecutionContext invocationContext =
                compiled.CreateExecutionContext(caller, [(BadObject)new BadNumber(42)]);

            Assert.That(invocationContext.Scope.HasLocal("input", invocationContext.Scope, false), Is.True);
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseSlotLocalFastPath = oldValue;
        }
    }

    [Test]
    public void CompiledFunctionTemplate_WithNestedFunction_BuildsSymbolTableWithoutLeakingInnerLocals()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("innerLocal", s_Position),
                                                                  new BadNumberExpression(5, s_Position),
                                                                  s_Position),
                                          new BadReturnExpression(new BadVariableExpression("innerLocal", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("outerLocal", s_Position),
                                                                  new BadNumberExpression(1, s_Position),
                                                                  s_Position),
                                          innerFunction,
                                          new BadReturnExpression(new BadVariableExpression("outerLocal", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        Assert.That(outerTemplate.SymbolTable, Is.Not.Null);
        Assert.That(outerTemplate.SymbolTable!.TryGetSymbol("outerLocal", out _), Is.True);
        Assert.That(outerTemplate.SymbolTable.TryGetSymbol("innerLocal", out _), Is.False);
        Assert.That(outerTemplate.SymbolTable.TryGetSymbol("Inner", out _), Is.False);
    }

    [Test]
    public void CompiledFunctionTemplate_ClosureSeesUpdatedOuterVariable()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [new BadReturnExpression(new BadVariableExpression("x", s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("x", s_Position),
                                                                  new BadNumberExpression(10, s_Position),
                                                                  s_Position),
                                          innerFunction,
                                          new BadAssignExpression(new BadVariableExpression("x", s_Position),
                                                                  new BadNumberExpression(25, s_Position),
                                                                  s_Position),
                                          new BadReturnExpression(new BadVariableExpression("Inner", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject outerCreated = BadObject.Null;
        foreach (BadObject o in outerTemplate.Instantiate(definitionContext))
        {
            outerCreated = o;
        }

        BadCompiledFunction outerCompiled = (BadCompiledFunction)outerCreated;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject innerCreated = BadObject.Null;
        foreach (BadObject o in outerCompiled.Invoke([], caller))
        {
            innerCreated = o;
        }

        BadCompiledFunction innerCompiled = (BadCompiledFunction)innerCreated;
        BadObject result = BadObject.Null;
        foreach (BadObject o in innerCompiled.Invoke([], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(25));
    }

    [Test]
    public void CompiledFunctionTemplate_ClosureCanMutateOuterVariable()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableExpression("x", s_Position),
                                                                  new BadNumberExpression(99, s_Position),
                                                                  s_Position),
                                          new BadReturnExpression(new BadVariableExpression("x", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("x", s_Position),
                                                                  new BadNumberExpression(10, s_Position),
                                                                  s_Position),
                                          innerFunction,
                                          new BadReturnExpression(new BadVariableExpression("Inner", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject outerCreated = BadObject.Null;
        foreach (BadObject o in outerTemplate.Instantiate(definitionContext))
        {
            outerCreated = o;
        }

        BadCompiledFunction outerCompiled = (BadCompiledFunction)outerCreated;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject innerCreated = BadObject.Null;
        foreach (BadObject o in outerCompiled.Invoke([], caller))
        {
            innerCreated = o;
        }

        BadCompiledFunction innerCompiled = (BadCompiledFunction)innerCreated;
        BadObject result = BadObject.Null;
        foreach (BadObject o in innerCompiled.Invoke([], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(99));
    }

    [Test]
    public void CompiledFunctionTemplate_InnerLocalShadowsOuterCapture()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("x", s_Position),
                                                                  new BadNumberExpression(5, s_Position),
                                                                  s_Position),
                                          new BadReturnExpression(new BadVariableExpression("x", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("x", s_Position),
                                                                  new BadNumberExpression(10, s_Position),
                                                                  s_Position),
                                          innerFunction,
                                          new BadReturnExpression(new BadVariableExpression("Inner", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject outerCreated = BadObject.Null;
        foreach (BadObject o in outerTemplate.Instantiate(definitionContext))
        {
            outerCreated = o;
        }

        BadCompiledFunction outerCompiled = (BadCompiledFunction)outerCreated;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject innerCreated = BadObject.Null;
        foreach (BadObject o in outerCompiled.Invoke([], caller))
        {
            innerCreated = o;
        }

        BadCompiledFunction innerCompiled = (BadCompiledFunction)innerCreated;
        BadObject result = BadObject.Null;
        foreach (BadObject o in innerCompiled.Invoke([], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(5));
    }

    [Test]
    public void CompiledFunctionTemplate_ClosureCapturesOuterParameter()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      [new BadReturnExpression(new BadVariableExpression("input", s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      ["input"],
                                      [
                                          innerFunction,
                                          new BadReturnExpression(new BadVariableExpression("Inner", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject outerCreated = BadObject.Null;
        foreach (BadObject o in outerTemplate.Instantiate(definitionContext))
        {
            outerCreated = o;
        }

        BadCompiledFunction outerCompiled = (BadCompiledFunction)outerCreated;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject innerCreated = BadObject.Null;
        foreach (BadObject o in outerCompiled.Invoke([(BadObject)new BadNumber(123)], caller))
        {
            innerCreated = o;
        }

        BadCompiledFunction innerCompiled = (BadCompiledFunction)innerCreated;
        BadObject result = BadObject.Null;
        foreach (BadObject o in innerCompiled.Invoke([], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(123));
    }

    [Test]
    public void CompiledFunctionTemplate_InnerParameterShadowsOuterCapture()
    {
        BadFunctionExpression innerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      ["x"],
                                      [new BadReturnExpression(new BadVariableExpression("x", s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadFunctionExpression outerFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Outer"),
                                      [],
                                      [
                                          new BadAssignExpression(new BadVariableDefinitionExpression("x", s_Position),
                                                                  new BadNumberExpression(10, s_Position),
                                                                  s_Position),
                                          innerFunction,
                                          new BadReturnExpression(new BadVariableExpression("Inner", s_Position), s_Position, false),
                                      ],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(outerFunction);
        BadCompiledFunctionTemplate outerTemplate = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject outerCreated = BadObject.Null;
        foreach (BadObject o in outerTemplate.Instantiate(definitionContext))
        {
            outerCreated = o;
        }

        BadCompiledFunction outerCompiled = (BadCompiledFunction)outerCreated;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject innerCreated = BadObject.Null;
        foreach (BadObject o in outerCompiled.Invoke([], caller))
        {
            innerCreated = o;
        }

        BadCompiledFunction innerCompiled = (BadCompiledFunction)innerCreated;
        BadObject result = BadObject.Null;
        foreach (BadObject o in innerCompiled.Invoke([(BadObject)new BadNumber(7)], caller))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(7));
    }

    [Test]
    public void CompiledFunctionTemplate_RecursiveFunctionReadsCurrentParameterValue()
    {
        BadFunctionExpression functionExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                      ["input"],
                                      [new BadReturnExpression(new BadVariableExpression("input", s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadExpressionCompileContext compileContext = new BadExpressionCompileContext(BadCompiler.Instance);
        compileContext.Compile(functionExpression);
        BadCompiledFunctionTemplate template = (BadCompiledFunctionTemplate)compileContext.GetInstructions()[0].Arguments[0];

        using BadExecutionContext definitionContext = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject created = BadObject.Null;
        foreach (BadObject o in template.Instantiate(definitionContext))
        {
            created = o;
        }

        BadCompiledFunction compiled = (BadCompiledFunction)created;
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadObject first = BadObject.Null;
        foreach (BadObject o in compiled.Invoke([(BadObject)new BadNumber(1)], caller))
        {
            first = o;
        }

        BadObject second = BadObject.Null;
        foreach (BadObject o in compiled.Invoke([(BadObject)new BadNumber(2)], caller))
        {
            second = o;
        }

        Assert.That(first, Is.TypeOf<BadNumber>());
        Assert.That(second, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)first).Value, Is.EqualTo(1));
        Assert.That(((IBadNumber)second).Value, Is.EqualTo(2));
    }

    [Test]
    public void InvocationExpression_WithVariableTarget_CompilesToInvoke()
    {
        BadInvocationExpression invocation =
            new BadInvocationExpression(new BadVariableExpression("fn", s_Position), [new BadNumberExpression(42, s_Position)], s_Position);
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(invocation);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions.Last().OpCode, Is.EqualTo(BadOpCode.Invoke));
    }

    [Test]
    public void InvocationExpression_WithMemberTarget_CompilesToInvokeMember()
    {
        BadMemberAccessExpression member = new BadMemberAccessExpression(new BadVariableExpression("obj", s_Position),
                                                                          BadWordToken.MakeWord("Run"),
                                                                          s_Position,
                                                                          [],
                                                                          false
                                                                         );
        BadInvocationExpression invocation =
            new BadInvocationExpression(member, [new BadNumberExpression(5, s_Position)], s_Position);
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(invocation);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions.Last().OpCode, Is.EqualTo(BadOpCode.InvokeMember));
        Assert.That(instructions.Last().Arguments[1], Is.EqualTo("Run"));
    }

    [Test]
    public void InvokeMemberInstruction_RuntimeInvokesTargetMember()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadExpressionFunction methodFunction = new BadExpressionFunction(context.Scope,
                                                                         BadWordToken.MakeWord("Run"),
                                                                         [new BadReturnExpression(new BadNumberExpression(11, s_Position), s_Position, false)],
                                                                         [],
                                                                         s_Position,
                                                                         false,
                                                                         false,
                                                                         null,
                                                                         BadAnyPrototype.Instance,
                                                                         false
                                                                        );
        BadTable target = new BadTable(new Dictionary<string, BadObject>
        {
            { "Run", methodFunction },
        });

        BadInstruction[] instructions =
        [
            new BadInstruction(BadOpCode.Push, s_Position, target),
            new BadInstruction(BadOpCode.InvokeMember, s_Position, 0, "Run", false),
        ];
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        BadObject result = BadObject.Null;

        foreach (BadObject o in vm.Execute(context))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(11));
    }

    [Test]
    public void ClassExpression_CompilesToCreateClassInstruction()
    {
        BadClassPrototypeExpression expression = new BadClassPrototypeExpression("TestClass",
                                                                                  [],
                                                                                  [],
                                                                                  [],
                                                                                  s_Position,
                                                                                  null,
                                                                                  []
                                                                                 );
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(expression);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions, Has.Length.EqualTo(1));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.CreateClass));
    }

    [Test]
    public void ClassExpression_TemplateCategorizesMembers()
    {
        BadFunctionExpression constructorExpression =
            new BadFunctionExpression(BadWordToken.MakeWord(BadStaticKeys.CONSTRUCTOR_NAME),
                                      [],
                                      [new BadReturnExpression(new BadNullExpression(s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadFunctionExpression methodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("GetNumber"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(7, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadNumberExpression(5, s_Position)
                                               );
        BadVariableDefinitionExpression fieldExpression = new BadVariableDefinitionExpression("Field", s_Position);
        BadFunctionExpression staticMethodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("StaticMethod"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(1, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      true,
                                      BadFunctionCompileLevel.None
                                     );
        BadClassPrototypeExpression expression = new BadClassPrototypeExpression("TestClass",
                                                                                  [constructorExpression, methodExpression, propertyExpression, fieldExpression],
                                                                                  [staticMethodExpression],
                                                                                  [],
                                                                                  s_Position,
                                                                                  null,
                                                                                  []
                                                                                 );
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(expression);
        BadInstruction[] instructions = context.GetInstructions();
        BadCompiledClassTemplate template = (BadCompiledClassTemplate)instructions[0].Arguments[0];

        Assert.That(template.InstanceMembers.Select(x => x.Kind),
                    Is.EqualTo(new[]
                    {
                        BadCompiledClassMemberKind.Constructor,
                        BadCompiledClassMemberKind.Method,
                        BadCompiledClassMemberKind.Property,
                        BadCompiledClassMemberKind.Field,
                    })
                   );
        Assert.That(template.InstanceMembers.Select(x => x.Name), Is.EqualTo(new[] { BadStaticKeys.CONSTRUCTOR_NAME, "GetNumber", "Value", "Field" }));
        Assert.That(template.StaticMembers.Select(x => x.Kind), Is.EqualTo(new[] { BadCompiledClassMemberKind.Method }));
        Assert.That(template.StaticMembers.All(x => x.IsStatic), Is.True);
        BadCompiledMethodTemplate constructorTemplate = template.InstanceMembers[0].Method!;
        BadCompiledMethodTemplate methodTemplate = template.InstanceMembers[1].Method!;
        Assert.That(constructorTemplate, Is.Not.Null);
        Assert.That(constructorTemplate.IsConstructor, Is.True);
        Assert.That(constructorTemplate.IsStatic, Is.False);
        Assert.That(methodTemplate, Is.Not.Null);
        Assert.That(methodTemplate.Name, Is.EqualTo("GetNumber"));
        Assert.That(methodTemplate.IsConstructor, Is.False);
        Assert.That(methodTemplate.Function.Expression.CompileLevel, Is.EqualTo(BadFunctionCompileLevel.None));
        BadCompiledPropertyTemplate propertyTemplate = template.InstanceMembers.Single(x => x.Kind == BadCompiledClassMemberKind.Property).Property!;
        Assert.That(propertyTemplate, Is.Not.Null);
        Assert.That(propertyTemplate.Name, Is.EqualTo("Value"));
        Assert.That(propertyTemplate.Getter, Is.Not.Null);
        Assert.That(propertyTemplate.Setter, Is.Null);
        BadCompiledMethodTemplate staticMethodTemplate = template.StaticMembers.Single().Method!;
        Assert.That(staticMethodTemplate.IsStatic, Is.True);
    }

    [Test]
    public void InterfaceExpression_CompilesToCreateInterfaceInstruction()
    {
        BadInterfacePrototypeExpression expression = new BadInterfacePrototypeExpression("ITest",
                                                                                          Array.Empty<BadInterfaceConstraint>(),
                                                                                          [],
                                                                                          null,
                                                                                          s_Position,
                                                                                          []
                                                                                         );
        BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

        context.Compile(expression);
        BadInstruction[] instructions = context.GetInstructions();

        Assert.That(instructions, Has.Length.EqualTo(1));
        Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.CreateInterface));
    }

    [Test]
    public void PropertyAccessorTemplate_CompilesSimpleGetterExpression()
    {
        BadCompiledPropertyAccessorTemplate template = new BadCompiledPropertyAccessorTemplate(new BadVariableExpression("_value", s_Position));

        Assert.That(template.CompiledInstructions, Is.Not.Null);
        Assert.That(template.CompiledInstructions!.Count, Is.GreaterThan(0));
    }

    [Test]
    public void PropertyAccessorTemplate_ExecutesCompiledAccessorAgainstScope()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        context.Scope.DefineVariable("_value", new BadNumber(17));
        BadCompiledPropertyAccessorTemplate template = new BadCompiledPropertyAccessorTemplate(new BadVariableExpression("_value", s_Position));
        BadObject result = BadObject.Null;

        foreach (BadObject o in template.Execute(context))
        {
            result = o;
        }

        result = result.Dereference(s_Position);
        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(17));
    }

    [Test]
    public void PropertyAccessorTemplate_KeepsAstFallbackForNonSimpleExpression()
    {
        BadCompiledPropertyAccessorTemplate template =
            new BadCompiledPropertyAccessorTemplate(new BadArrayExpression(Array.Empty<BadExpression>(), s_Position));

        Assert.That(template.CompiledInstructions, Is.Null);
    }

    [Test]
    public void PropertyAccessorTemplate_ExecutesDirectSetterFastPath()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        context.Scope.DefineVariable("_value", new BadNumber(1));
        context.Scope.DefineVariable("value", new BadNumber(42));
        BadCompiledPropertyAccessorTemplate template =
            new BadCompiledPropertyAccessorTemplate(new BadAssignExpression(new BadVariableExpression("_value", s_Position),
                                                                            new BadVariableExpression("value", s_Position),
                                                                            s_Position
                                                                           ));

        foreach (BadObject _ in template.Execute(context))
        {
        }

        BadObject updated = context.Scope.GetVariable("_value", context.Scope).Dereference(s_Position);
        Assert.That(updated, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)updated).Value, Is.EqualTo(42));
    }

    [Test]
    public void PropertyAccessorTemplate_ExecutesDirectThisMemberGetterFastPath()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadTable thisObject = new BadTable();
        thisObject.SetProperty("Field", new BadNumber(99));
        context.Scope.DefineVariable(BadStaticKeys.THIS_KEY, thisObject);
        BadCompiledPropertyAccessorTemplate template =
            new BadCompiledPropertyAccessorTemplate(new BadMemberAccessExpression(new BadVariableExpression(BadStaticKeys.THIS_KEY, s_Position),
                                                                                  BadWordToken.MakeWord("Field"),
                                                                                  s_Position,
                                                                                  []
                                                                                 ));
        BadObject result = BadObject.Null;

        foreach (BadObject o in template.Execute(context))
        {
            result = o;
        }

        result = result.Dereference(s_Position);
        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(99));
    }

    [Test]
    public void PropertyAccessorTemplate_ExecutesDirectThisMemberSetterFastPath()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadTable thisObject = new BadTable();
        thisObject.SetProperty("Field", new BadNumber(0));
        context.Scope.DefineVariable(BadStaticKeys.THIS_KEY, thisObject);
        context.Scope.DefineVariable("value", new BadNumber(123));
        BadCompiledPropertyAccessorTemplate template =
            new BadCompiledPropertyAccessorTemplate(new BadAssignExpression(new BadMemberAccessExpression(new BadVariableExpression(BadStaticKeys.THIS_KEY, s_Position),
                                                                                                           BadWordToken.MakeWord("Field"),
                                                                                                           s_Position,
                                                                                                           []
                                                                                                          ),
                                                                            new BadVariableExpression("value", s_Position),
                                                                            s_Position
                                                                           ));

        foreach (BadObject _ in template.Execute(context))
        {
        }

        BadObject updated = thisObject.GetProperty("Field", context.Scope).Dereference(s_Position);
        Assert.That(updated, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)updated).Value, Is.EqualTo(123));
    }

    [Test]
    public void PropertyExpression_ParsesAccessorCompileKeywords()
    {
        const string source = "class C { let num __counter = 0; let num Counter { compiled fast get => __counter++; compiled fast set => __counter = value; } }";
        BadClassPrototypeExpression classExpression = BadRuntime.Parse(source, "<test>").OfType<BadClassPrototypeExpression>().Single();
        BadPropertyDefinitionExpression propertyExpression = classExpression.Body.OfType<BadPropertyDefinitionExpression>().Single();

        Assert.That(propertyExpression.GetCompileLevel, Is.EqualTo(BadFunctionCompileLevel.CompiledFast));
        Assert.That(propertyExpression.SetCompileLevel, Is.EqualTo(BadFunctionCompileLevel.CompiledFast));
    }

    [Test]
    public void PropertyTemplate_UsesAccessorCompileKeywordsForForcedCompilation()
    {
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadArrayExpression(Array.Empty<BadExpression>(), s_Position),
                                                null,
                                                new BadArrayExpression(Array.Empty<BadExpression>(), s_Position),
                                                false,
                                                BadFunctionCompileLevel.Compiled,
                                                BadFunctionCompileLevel.CompiledFast
                                               );
        BadCompiledPropertyTemplate template = new BadCompiledPropertyTemplate(propertyExpression);

        Assert.That(template.Getter.CompiledInstructions, Is.Not.Null);
        Assert.That(template.Setter, Is.Not.Null);
        Assert.That(template.Setter!.CompiledInstructions, Is.Not.Null);
        Assert.That(template.Getter.UseOverrides, Is.True);
        Assert.That(template.Setter!.UseOverrides, Is.False);
    }

    [Test]
    public void PropertyExpression_ParsesMixedAccessorCompileKeywords()
    {
        const string source = "class C { let num __counter = 0; let num Counter { compiled get => __counter; legacy set => __counter = value; } }";
        BadClassPrototypeExpression classExpression = BadRuntime.Parse(source, "<test>").OfType<BadClassPrototypeExpression>().Single();
        BadPropertyDefinitionExpression propertyExpression = classExpression.Body.OfType<BadPropertyDefinitionExpression>().Single();

        Assert.That(propertyExpression.GetCompileLevel, Is.EqualTo(BadFunctionCompileLevel.Compiled));
        Assert.That(propertyExpression.SetCompileLevel, Is.EqualTo(BadFunctionCompileLevel.None));
    }

    [Test]
    public void PropertyExpression_ParsesCompiledFastSetWithDefaultGetter()
    {
        const string source = "class C { let num __counter = 0; let num Counter { get => __counter; compiled fast set => __counter = value; } }";
        BadClassPrototypeExpression classExpression = BadRuntime.Parse(source, "<test>").OfType<BadClassPrototypeExpression>().Single();
        BadPropertyDefinitionExpression propertyExpression = classExpression.Body.OfType<BadPropertyDefinitionExpression>().Single();

        Assert.That(propertyExpression.GetCompileLevel, Is.EqualTo(BadFunctionCompileLevel.None));
        Assert.That(propertyExpression.SetCompileLevel, Is.EqualTo(BadFunctionCompileLevel.CompiledFast));
    }

    [Test]
    public void PropertyTemplate_UsesMixedAccessorCompileLevels()
    {
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadArrayExpression(Array.Empty<BadExpression>(), s_Position),
                                                null,
                                                new BadArrayExpression(Array.Empty<BadExpression>(), s_Position),
                                                false,
                                                BadFunctionCompileLevel.Compiled,
                                                BadFunctionCompileLevel.None
                                               );
        BadCompiledPropertyTemplate template = new BadCompiledPropertyTemplate(propertyExpression);

        Assert.That(template.Getter.CompiledInstructions, Is.Not.Null);
        Assert.That(template.Getter.UseOverrides, Is.True);
        Assert.That(template.Setter, Is.Not.Null);
        Assert.That(template.Setter!.CompiledInstructions, Is.Null);
    }

    [Test]
    public void PropertyExpression_ThrowsWhenCompileModifierHasNoSetAccessor()
    {
        const string source = "class C { let num __counter = 0; let num Counter { get => __counter; compiled fast } }";

        Assert.Throws<BadParserException>(() => BadRuntime.Parse(source, "<test>").ToArray());
    }

    [Test]
    public void CreateClassInstruction_RuntimeCreatesUsableClassFromStructuredMembers()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadFunctionExpression methodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("GetNumber"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(7, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadNumberExpression(5, s_Position)
                                               );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [methodExpression, propertyExpression],
                                                                                       [],
                                                                                       [],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
        BadObject instance = BadObject.Null;

        foreach (BadObject o in prototype.CreateInstance(context))
        {
            instance = o;
        }

        Assert.That(instance, Is.TypeOf<BadClass>());
        BadClass cls = (BadClass)instance;

        BadObject propertyValue = cls.GetProperty("Value", context.Scope).Dereference(s_Position);
        Assert.That(propertyValue, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)propertyValue).Value, Is.EqualTo(5));

        BadFunction method = (BadFunction)cls.GetProperty("GetNumber", context.Scope).Dereference(s_Position);
        BadObject methodResult = BadObject.Null;

        foreach (BadObject o in method.Invoke([], context))
        {
            methodResult = o;
        }

        Assert.That(methodResult, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)methodResult).Value, Is.EqualTo(7));
    }

    [Test]
    public void CreateClassInstruction_RuntimeUsesStructuredPropertySetterTemplate()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadVariableExpression("_value", s_Position),
                                                null,
                                                new BadAssignExpression(new BadVariableExpression("_value", s_Position),
                                                                        new BadVariableExpression("value", s_Position),
                                                                        s_Position
                                                                       )
                                               );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [new BadVariableDefinitionExpression("_value", s_Position), propertyExpression],
                                                                                       [],
                                                                                       [],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
        BadClass cls = (BadClass)prototype.CreateInstance(context).Last();

        cls.GetProperty("Value", context.Scope).Set(new BadNumber(42), s_Position);

        BadObject propertyValue = cls.GetProperty("Value", context.Scope).Dereference(s_Position);
        Assert.That(propertyValue, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)propertyValue).Value, Is.EqualTo(42));
    }

    [Test]
    public void CreateClassInstruction_RuntimeUsesStructuredStaticCompiledMethodTemplate()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadFunctionExpression staticMethodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("StaticMethod"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(11, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      true,
                                      BadFunctionCompileLevel.Compiled
                                     );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [],
                                                                                       [staticMethodExpression],
                                                                                       [],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
        BadCompiledClassTemplate template = (BadCompiledClassTemplate)instructions[0].Arguments[0];

        Assert.That(template.StaticMembers.Single().Method, Is.Not.Null);
        Assert.That(template.StaticMembers.Single().Method!.Function.CompiledInstructions, Is.Not.Null);
        Assert.That(template.StaticMembers.Single().Method!.Function.UseOverrides, Is.True);

        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
        BadFunction staticMethod = (BadFunction)prototype.GetProperty("StaticMethod", context.Scope).Dereference(s_Position);
        BadObject result = BadObject.Null;

        foreach (BadObject o in staticMethod.Invoke([], context))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(11));
    }

    [Test]
    public void StaticMembers_AreMaterializedInDedicatedStaticOrder()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        List<string> log = new List<string>();
        BadExpressionClassPrototype? prototype = null;
        Action<BadCompiledClassMemberTemplate> handler = member => log.Add(member.Kind.ToString().ToLowerInvariant());
        BadExpressionClassPrototype.OnMaterializeStaticMember += handler;

        try
        {
            BadFunctionExpression staticMethodExpression =
                new BadFunctionExpression(BadWordToken.MakeWord("StaticMethod"),
                                          [],
                                          [new BadReturnExpression(new BadNumberExpression(11, s_Position), s_Position, false)],
                                          s_Position,
                                          false,
                                          null,
                                          false,
                                          true,
                                          BadFunctionCompileLevel.None
                                         );
            BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                           [],
                                                                                           [new BadPropertyDefinitionExpression(BadWordToken.MakeWord("StaticValue"),
                                                                                                                               s_Position,
                                                                                                                               new BadNumberExpression(7, s_Position)
                                                                                                                              ),
                                                                                            staticMethodExpression,
                                                                                            new BadVariableDefinitionExpression("StaticField", s_Position),
                                                                                            new BadStringExpression("Ignored", s_Position)
                                                                                           ],
                                                                                           [],
                                                                                           s_Position,
                                                                                           null,
                                                                                           []
                                                                                          );
            BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                   true,
                                                                   context.Scope,
                                                                   s_Position,
                                                                   BadWordToken.MakeWord("Test"),
                                                                   false,
                                                                   false,
                                                                   null,
                                                                   BadAnyPrototype.Instance,
                                                                   false
                                                                  );
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

            foreach (BadObject _ in vm.Execute(context))
            {
            }

            prototype = (BadExpressionClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
            BadObject staticValue = prototype.GetProperty("StaticValue", context.Scope).Dereference(s_Position);

            Assert.That(staticValue, Is.TypeOf<BadNumber>());
            Assert.That(((IBadNumber)staticValue).Value, Is.EqualTo(7));
        }
        finally
        {
            BadExpressionClassPrototype.OnMaterializeStaticMember -= handler;
        }

        Assert.That(log, Is.EqualTo(new[] { "field", "unknown", "method", "property" }));
        Assert.That(prototype, Is.Not.Null);
    }

    [Test]
    public void CreateInstance_BindsBaseBeforeMemberMaterialization()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadClassPrototypeExpression baseClassExpression = new BadClassPrototypeExpression("BaseClass",
                                                                                           [],
                                                                                           [],
                                                                                           [],
                                                                                           s_Position,
                                                                                           null,
                                                                                           []
                                                                                          );
        BadVariableDefinitionExpression hasBaseDefinition = new BadVariableDefinitionExpression("HasBase", s_Position);
        BadAssignExpression assignHasBase = new BadAssignExpression(new BadVariableExpression("HasBase", s_Position),
                                                                    new BadInequalityExpression(new BadVariableExpression(BadStaticKeys.BASE_KEY, s_Position),
                                                                                                new BadNullExpression(s_Position),
                                                                                                s_Position
                                                                                               ),
                                                                    s_Position
                                                                   );
        BadClassPrototypeExpression derivedClassExpression = new BadClassPrototypeExpression("DerivedClass",
                                                                                              [hasBaseDefinition, assignHasBase],
                                                                                              [],
                                                                                              [new BadVariableExpression("BaseClass", s_Position)],
                                                                                              s_Position,
                                                                                              null,
                                                                                              []
                                                                                             );
        BadInstruction[] instructions = BadCompiler.Compile([baseClassExpression, derivedClassExpression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype derivedPrototype = (BadClassPrototype)context.Scope.GetVariable("DerivedClass").Dereference(s_Position);
        BadClass derivedInstance = (BadClass)derivedPrototype.CreateInstance(context).Last();
        BadObject hasBase = derivedInstance.GetProperty("HasBase", context.Scope).Dereference(s_Position);

        Assert.That(hasBase, Is.EqualTo(BadObject.True));
    }

    [Test]
    public void CreateInstance_ValidatesInterfacesAfterMemberMaterialization()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadInterfacePrototypeExpression interfaceExpression = new BadInterfacePrototypeExpression("ITest",
                                                                                                   [new BadInterfaceFunctionConstraint("GetNumber", null, [])],
                                                                                                   [],
                                                                                                   null,
                                                                                                   s_Position,
                                                                                                   []
                                                                                                  );
        BadFunctionExpression methodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("GetNumber"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(23, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [methodExpression],
                                                                                       [],
                                                                                       [new BadVariableExpression("ITest", s_Position)],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadInstruction[] instructions = BadCompiler.Compile([interfaceExpression, classExpression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
        BadClass instance = (BadClass)prototype.CreateInstance(context).Last();
        BadFunction method = (BadFunction)instance.GetProperty("GetNumber", context.Scope).Dereference(s_Position);
        BadObject result = BadObject.Null;

        foreach (BadObject o in method.Invoke([], context))
        {
            result = o;
        }

        Assert.That(result, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)result).Value, Is.EqualTo(23));
    }

    [Test]
    public void CreateInstance_MaterializesMembersInFieldMethodPropertyConstructorOrder()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        List<string> log = new List<string>();
        BadFunctionExpression constructorExpression =
            new BadFunctionExpression(BadWordToken.MakeWord(BadStaticKeys.CONSTRUCTOR_NAME),
                                      [],
                                      [new BadReturnExpression(new BadNullExpression(s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadNumberExpression(1, s_Position)
                                               );
        BadVariableDefinitionExpression fieldExpression = new BadVariableDefinitionExpression("Field", s_Position);
        BadFunctionExpression methodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("GetNumber"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(1, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [constructorExpression, propertyExpression, fieldExpression, methodExpression],
                                                                                       [],
                                                                                       [],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);

        void OnMaterialize(BadCompiledClassMemberTemplate member)
        {
            log.Add(member.Kind.ToString().ToLowerInvariant());
        }

        BadExpressionClassPrototype.OnMaterializeInstanceMember += OnMaterialize;

        try
        {
            _ = prototype.CreateInstance(context).Last();
        }
        finally
        {
            BadExpressionClassPrototype.OnMaterializeInstanceMember -= OnMaterialize;
        }

        Assert.That(log, Is.EqualTo(new[] { "field", "method", "property", "constructor" }));
    }

    [Test]
    public void CreateObject_InvokesConstructorAfterMemberSubphases()
    {
        using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadVariableDefinitionExpression backingField = new BadVariableDefinitionExpression("_value", s_Position);
        BadFunctionExpression methodExpression =
            new BadFunctionExpression(BadWordToken.MakeWord("GetInitial"),
                                      [],
                                      [new BadReturnExpression(new BadNumberExpression(99, s_Position), s_Position, false)],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadPropertyDefinitionExpression propertyExpression =
            new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                s_Position,
                                                new BadVariableExpression("_value", s_Position),
                                                null,
                                                new BadAssignExpression(new BadVariableExpression("_value", s_Position),
                                                                        new BadVariableExpression("value", s_Position),
                                                                        s_Position
                                                                       )
                                               );
        BadMemberAccessExpression propertyAccess = new BadMemberAccessExpression(new BadVariableExpression(BadStaticKeys.THIS_KEY, s_Position),
                                                                                 BadWordToken.MakeWord("Value"),
                                                                                 s_Position,
                                                                                 []
                                                                                );
        BadMemberAccessExpression methodAccess = new BadMemberAccessExpression(new BadVariableExpression(BadStaticKeys.THIS_KEY, s_Position),
                                                                               BadWordToken.MakeWord("GetInitial"),
                                                                               s_Position,
                                                                               []
                                                                              );
        BadFunctionExpression constructorExpression =
            new BadFunctionExpression(BadWordToken.MakeWord(BadStaticKeys.CONSTRUCTOR_NAME),
                                      [],
                                      [new BadAssignExpression(propertyAccess,
                                                               new BadInvocationExpression(methodAccess, [], s_Position),
                                                               s_Position
                                                              )],
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [constructorExpression, propertyExpression, backingField, methodExpression],
                                                                                       [],
                                                                                       [],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }

        BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
        BadObject created = BadObject.Null;

        foreach (BadObject o in BadNewExpression.CreateObject(prototype, context, [], s_Position))
        {
            created = o;
        }

        BadClass cls = (BadClass)created;
        BadObject value = cls.GetProperty("Value", context.Scope).Dereference(s_Position);

        Assert.That(value, Is.TypeOf<BadNumber>());
        Assert.That(((IBadNumber)value).Value, Is.EqualTo(99));
    }

    [Test]
    public void EvalInstructionCounter_RemainsZeroForCompiledFunctionExpression()
    {
        List<BadExpression> innerBody =
        [
            new BadReturnExpression(new BadNumberExpression(1, s_Position), s_Position, false)
        ];
        BadFunctionExpression nestedFunction =
            new BadFunctionExpression(BadWordToken.MakeWord("Inner"),
                                      [],
                                      innerBody,
                                      s_Position,
                                      false,
                                      null,
                                      false,
                                      false,
                                      BadFunctionCompileLevel.None
                                     );
        BadExpressionFunction outerFunction =
            new BadExpressionFunction(new BadScope("Test", new BadInteropExtensionProvider()),
                                      BadWordToken.MakeWord("Outer"),
                                      [nestedFunction],
                                      [],
                                      s_Position,
                                      false,
                                      false,
                                      null,
                                      BadAnyPrototype.Instance,
                                      false
                                     );
        BadCompiledFunction compiled = BadCompilerApi.CompileFunction(outerFunction, true);
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadRuntimeVirtualMachine.ResetEvalInstructionCounter();

        foreach (BadObject _ in compiled.Invoke([], caller))
        {
        }

        Assert.That(BadRuntimeVirtualMachine.EvalInstructionCount, Is.EqualTo(0));
    }

    [Test]
    public void EvalInstructionCounter_RemainsZeroForCompiledClassAndInterfaceExpressions()
    {
        BadInterfacePrototypeExpression interfaceExpression = new BadInterfacePrototypeExpression("ITest",
                                                                                                   Array.Empty<BadInterfaceConstraint>(),
                                                                                                   [],
                                                                                                   null,
                                                                                                   s_Position,
                                                                                                   []
                                                                                                  );
        BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                       [],
                                                                                       [],
                                                                                       [new BadVariableExpression("ITest", s_Position)],
                                                                                       s_Position,
                                                                                       null,
                                                                                       []
                                                                                      );
        BadExpressionFunction outerFunction =
            new BadExpressionFunction(new BadScope("Test", new BadInteropExtensionProvider()),
                                      BadWordToken.MakeWord("Outer"),
                                      [interfaceExpression, classExpression],
                                      [],
                                      s_Position,
                                      false,
                                      false,
                                      null,
                                      BadAnyPrototype.Instance,
                                      false
                                     );
        BadCompiledFunction compiled = BadCompilerApi.CompileFunction(outerFunction, true);
        using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());

        BadRuntimeVirtualMachine.ResetEvalInstructionCounter();

        foreach (BadObject _ in compiled.Invoke([], caller))
        {
        }

         Assert.That(BadRuntimeVirtualMachine.EvalInstructionCount, Is.EqualTo(0));
     }

     [Test]
     public void CreateClassTemplate_StructuresFieldsAsExplicitTemplates()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadVariableDefinitionExpression fieldExpression = new BadVariableDefinitionExpression("MyField", s_Position);
         BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                        [fieldExpression],
                                                                                        [],
                                                                                        [],
                                                                                        s_Position,
                                                                                        null,
                                                                                        []
                                                                                       );
         BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
         BadCompiledClassTemplate template = (BadCompiledClassTemplate)instructions[0].Arguments[0];

         Assert.That(template.InstanceMembers.Count, Is.EqualTo(1));
         BadCompiledClassMemberTemplate member = template.InstanceMembers.First();
         Assert.That(member.Kind, Is.EqualTo(BadCompiledClassMemberKind.Field));
         Assert.That(member.Field, Is.Not.Null);
         Assert.That(member.Name, Is.EqualTo("MyField"));
     }

     [Test]
     public void CreateClassInstruction_RuntimeUsesStructuredFieldTemplate()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadVariableDefinitionExpression fieldExpression = new BadVariableDefinitionExpression("Counter", s_Position);
         BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                        [fieldExpression],
                                                                                        [],
                                                                                        [],
                                                                                        s_Position,
                                                                                        null,
                                                                                        []
                                                                                       );
         BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
         BadClass instance = (BadClass)prototype.CreateInstance(context).Last();

         Assert.That(instance.HasProperty("Counter", context.Scope), Is.True);
     }

      [Test]
      public void ForEachExpression_CompilesToDirectLoopInstructions_WithoutEvalFallback()
      {
          BadForEachExpression expression = new BadForEachExpression(new BadArrayExpression([new BadNumberExpression(1, s_Position),
                                                                                             new BadNumberExpression(2, s_Position),
                                                                                             new BadNumberExpression(3, s_Position)],
                                                                                           s_Position),
                                                              BadWordToken.MakeWord("item"),
                                                              [new BadAddAssignExpression(new BadVariableExpression("sum", s_Position),
                                                                                         new BadVariableExpression("item", s_Position),
                                                                                         s_Position
                                                                                        )],
                                                              s_Position
                                                             );

          BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

          context.Compile(expression);
          BadInstruction[] instructions = context.GetInstructions();

          Assert.That(instructions.Any(x => x.OpCode == BadOpCode.Eval), Is.False);
          Assert.That(instructions.Any(x => x.OpCode == BadOpCode.BeginLoop), Is.True);
          Assert.That(instructions.Any(x => x.OpCode == BadOpCode.EndLoop), Is.True);
            Assert.That(instructions.Any(x => x.OpCode == BadOpCode.GetEnumerator), Is.True);
            Assert.That(instructions.Any(x => x.OpCode == BadOpCode.MoveNext), Is.True);
            Assert.That(instructions.Any(x => x.OpCode == BadOpCode.GetCurrent), Is.True);
          Assert.That(instructions.Any(x => x.OpCode == BadOpCode.CreateScope), Is.True);
          Assert.That(instructions.Any(x => x.OpCode == BadOpCode.DestroyScope), Is.True);
      }

      [Test]
      public void ForEachExpression_RuntimeSummation_UsesDirectCompiledLoop()
      {
          using BadExecutionContext caller = BadExecutionContext.Create(new BadInteropExtensionProvider());

          BadForEachExpression foreachExpression = new BadForEachExpression(new BadArrayExpression([new BadNumberExpression(1, s_Position),
                                                                                                   new BadNumberExpression(2, s_Position),
                                                                                                   new BadNumberExpression(3, s_Position)],
                                                                                                 s_Position),
                                                                      BadWordToken.MakeWord("item"),
                                                                      [new BadAddAssignExpression(new BadVariableExpression("sum", s_Position),
                                                                                                 new BadVariableExpression("item", s_Position),
                                                                                                 s_Position
                                                                                                )],
                                                                      s_Position
                                                                     );

          BadExpressionFunction function = new BadExpressionFunction(caller.Scope,
                                                                     BadWordToken.MakeWord("SumLoop"),
                                                                       [new BadVariableDefinitionExpression("sum", s_Position),
                                                                        new BadAssignExpression(new BadVariableExpression("sum", s_Position),
                                                                                                new BadNumberExpression(0, s_Position),
                                                                                                s_Position),
                                                                        foreachExpression,
                                                                        new BadReturnExpression(new BadVariableExpression("sum", s_Position), s_Position, false)],
                                                                     [],
                                                                     s_Position,
                                                                     false,
                                                                     false,
                                                                     null,
                                                                     BadAnyPrototype.Instance,
                                                                     false
                                                                    );

          BadCompiledFunction compiled = BadCompilerApi.CompileFunction(function, true);
          BadRuntimeVirtualMachine.ResetEvalInstructionCounter();

            BadObject result = BadObject.Null;
            foreach (BadObject _ in compiled.Invoke([], caller))
            {
                result = _;
            }

            Assert.That(result, Is.TypeOf<BadNumber>());
            Assert.That(((IBadNumber)result).Value, Is.EqualTo(6));
          Assert.That(BadRuntimeVirtualMachine.EvalInstructionCount, Is.EqualTo(0));
      }

     [Test]
     public void Repro_CompiledTableReturn_ReturnsTable()
     {
         const string source = """
                               compiled function Copy()
                               {
                                   return {A: 1, B: 2};
                               }

                               let result = Copy();
                               """;

         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadExpression[] expressions = BadRuntime.Parse(source, "<repro>").ToArray();
         BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadObject result = context.Scope.GetVariable("result").Dereference(s_Position);

         Assert.That(result, Is.TypeOf<BadTable>());
         BadTable table = (BadTable)result;
         BadObject a = table.GetProperty("A", context.Scope).Dereference(s_Position);
         BadObject b = table.GetProperty("B", context.Scope).Dereference(s_Position);
         Assert.That(a, Is.TypeOf<BadNumber>());
         Assert.That(((BadNumber)a).Value, Is.EqualTo(1));
         Assert.That(b, Is.TypeOf<BadNumber>());
         Assert.That(((BadNumber)b).Value, Is.EqualTo(2));
     }
     
     [Test]
     public void Repro_CompiledForEachTableCopy_ReturnsCopiedTable()
     {
         const string source = """
                               %FNDEF% Copy(table)
                               {
                                   const t = {};
                                   t.A = table["A"];
                                   return t;
                               }

                               let result = Copy({A: 1});
                               """;

         var compiledSource = source.Replace("%FNDEF%", "compiled function");
         var uncompiledSource = source.Replace("%FNDEF%", "function");
         

         void TestSource(string name, BadExpression[] expressions)
         {
             using var runtime = new BadRuntime()
                 .UseCommonExtensions()
                 .UseCommonInterop();
             using var context = runtime.CreateContext(Directory.GetCurrentDirectory());
             BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
             BadCompiledFunction function = new BadCompiledFunction(instructions,
                 true,
                 context.Scope,
                 s_Position,
                 BadWordToken.MakeWord("Test"),
                 false,
                 false,
                 null,
                 BadAnyPrototype.Instance,
                 false
             );
             BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

             foreach (BadObject _ in vm.Execute(context))
             {
             }

             BadObject result = context.Scope.GetVariable("result").Dereference(s_Position);

             Assert.That(result, Is.TypeOf<BadTable>(), $"[{name}] Expected table, got {result.GetType()}");
             BadTable table = (BadTable)result;
             // BadObject a = table.GetProperty("A", context.Scope).Dereference(s_Position);
             // BadObject b = table.GetProperty("B", context.Scope).Dereference(s_Position);
             // Assert.That(a, Is.TypeOf<BadNumber>(), $"[{name}] Expected number, got {a.GetType()}");
             // Assert.That(((BadNumber)a).Value, Is.EqualTo(1), $"[{name}] Expected 1, got {a.GetType()}");
             // Assert.That(b, Is.TypeOf<BadNumber>(), $"[{name}] Expected number, got {b.GetType()}");
             // Assert.That(((BadNumber)b).Value, Is.EqualTo(2), $"[{name}] Expected 2, got {b.GetType()}");
         }
         
         BadExpression[] uncompiledExpressions = BadRuntime.Parse(uncompiledSource, "<repro>").ToArray();
         TestSource("uncompiled",uncompiledExpressions);

         BadExpression[] compiledExpressions = BadRuntime.Parse(compiledSource, "<repro>").ToArray();
         TestSource("compiled", compiledExpressions);
     }

     [Test]
     public void FunctionExpression_WithParameters_CompilesToCreateFunctionInstruction()
     {
         var param = new BadFunctionParameter("x", false, false, false);
         BadFunctionExpression expression =
             new BadFunctionExpression(BadWordToken.MakeWord("Fn"),
                                       [param],
                                       [new BadReturnExpression(new BadVariableExpression("x", s_Position), s_Position, false)],
                                       s_Position,
                                       false,
                                       null,
                                       false,
                                       false,
                                       BadFunctionCompileLevel.Compiled
                                      );
         BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

         context.Compile(expression);
         BadInstruction[] instructions = context.GetInstructions();

         Assert.That(instructions, Has.Length.EqualTo(1));
         Assert.That(instructions[0].OpCode, Is.EqualTo(BadOpCode.CreateFunction));
     }

     [Test]
     public void CompileExpressionSequence_Empty_ProducesNoInstructions()
     {
         BadExpressionCompileContext context = new BadExpressionCompileContext(BadCompiler.Instance);

         context.Compile(Array.Empty<BadExpression>());
         BadInstruction[] instructions = context.GetInstructions();

         Assert.That(instructions, Is.Empty);
     }

     [Test]
     public void CreateClassInstruction_TwoInstances_DoNotShareFieldState()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadVariableDefinitionExpression backingField = new BadVariableDefinitionExpression("_value", s_Position);
         BadPropertyDefinitionExpression propertyExpression =
             new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                 s_Position,
                                                 new BadVariableExpression("_value", s_Position),
                                                 null,
                                                 new BadAssignExpression(new BadVariableExpression("_value", s_Position),
                                                                         new BadVariableExpression("value", s_Position),
                                                                         s_Position
                                                                        )
                                                );
         BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                        [backingField, propertyExpression],
                                                                                        [],
                                                                                        [],
                                                                                        s_Position,
                                                                                        null,
                                                                                        []
                                                                                       );
         BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
         BadClass instance1 = (BadClass)prototype.CreateInstance(context).Last();
         BadClass instance2 = (BadClass)prototype.CreateInstance(context).Last();

         instance1.GetProperty("Value", context.Scope).Set(new BadNumber(10), s_Position);
         instance2.GetProperty("Value", context.Scope).Set(new BadNumber(20), s_Position);

         BadObject value1 = instance1.GetProperty("Value", context.Scope).Dereference(s_Position);
         BadObject value2 = instance2.GetProperty("Value", context.Scope).Dereference(s_Position);

         Assert.That(((IBadNumber)value1).Value, Is.EqualTo(10));
         Assert.That(((IBadNumber)value2).Value, Is.EqualTo(20));
     }

     [Test]
     public void CreateClassInstruction_MissingInterfaceConstraint_ThrowsOnInstantiation()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadInterfacePrototypeExpression interfaceExpression = new BadInterfacePrototypeExpression("ITest",
                                                                                                    [new BadInterfaceFunctionConstraint("RequiredMethod", null, [])],
                                                                                                    [],
                                                                                                    null,
                                                                                                    s_Position,
                                                                                                    []
                                                                                                   );
         BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                        [],
                                                                                        [],
                                                                                        [new BadVariableExpression("ITest", s_Position)],
                                                                                        s_Position,
                                                                                        null,
                                                                                        []
                                                                                       );
         BadInstruction[] instructions = BadCompiler.Compile([interfaceExpression, classExpression]).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);

         Assert.Throws<BadRuntimeException>(() => prototype.CreateInstance(context).Last());
     }

     [Test]
     public void CreateClassInstruction_CompiledMethodWithParameters_ReturnsCorrectResult()
     {
         const string source = """
                               class TestClass
                               {
                                   compiled function GetValue(x) { return x; }
                               }
                               let instance = new TestClass();
                               let result = instance.GetValue(42);
                               """;

         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
         BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadObject result = context.Scope.GetVariable("result").Dereference(s_Position);

         Assert.That(result, Is.TypeOf<BadNumber>());
         Assert.That(((IBadNumber)result).Value, Is.EqualTo(42));
     }

     [Test]
     public void PropertyAccessorTemplate_WithNumberConstant_CompilesAndExecutes()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadCompiledPropertyAccessorTemplate template =
             new BadCompiledPropertyAccessorTemplate(new BadNumberExpression(42, s_Position));

         Assert.That(template.CompiledInstructions, Is.Not.Null);

         BadObject result = BadObject.Null;

         foreach (BadObject o in template.Execute(context))
         {
             result = o;
         }

         result = result.Dereference(s_Position);
         Assert.That(result, Is.TypeOf<BadNumber>());
         Assert.That(((IBadNumber)result).Value, Is.EqualTo(42));
     }

     [Test]
     public void InvokeMemberInstruction_MethodWithArguments_ReturnsCorrectResult()
     {
         const string source = """
                               compiled function Add(a, b) { return a + b; }
                               let obj = { Add: Add };
                               let result = obj.Add(3, 4);
                               """;

         using BadRuntime runtime = new BadRuntime().UseCommonExtensions().UseCommonInterop();
         using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
         BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
         BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadObject result = context.Scope.GetVariable("result").Dereference(s_Position);

         Assert.That(result, Is.TypeOf<BadNumber>());
         Assert.That(((IBadNumber)result).Value, Is.EqualTo(7));
     }

     [Test]
     public void MethodSlot_IsPopulatedAfterInstanceCreation()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadFunctionExpression methodExpression =
             new BadFunctionExpression(BadWordToken.MakeWord("GetValue"),
                                       [],
                                       [new BadReturnExpression(new BadNumberExpression(42, s_Position), s_Position, false)],
                                       s_Position,
                                       false,
                                       null,
                                       false,
                                       false,
                                       BadFunctionCompileLevel.None
                                      );
         BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                        [methodExpression],
                                                                                        [],
                                                                                        [],
                                                                                        s_Position,
                                                                                        null,
                                                                                        []
                                                                                       );
         BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
         BadClass instance = (BadClass)prototype.CreateInstance(context).Last();

         Assert.That(instance.TryGetMethodSlot("GetValue", out BadObject? slotMethod), Is.True);
         Assert.That(slotMethod, Is.InstanceOf<BadFunction>());
     }

     [Test]
     public void MethodSlot_FastPathInvokesMethodCorrectly()
     {
         const string source = """
                               class TestClass
                               {
                                   compiled function GetNumber() { return 77; }
                               }
                               let instance = new TestClass();
                               let result = instance.GetNumber();
                               """;

         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
         BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadObject result = context.Scope.GetVariable("result").Dereference(s_Position);

         Assert.That(result, Is.TypeOf<BadNumber>());
         Assert.That(((IBadNumber)result).Value, Is.EqualTo(77));
     }

     [Test]
     public void MethodSlot_NotPopulatedForConstructorOrProperty()
     {
         using BadExecutionContext context = BadExecutionContext.Create(new BadInteropExtensionProvider());
         BadFunctionExpression constructorExpression =
             new BadFunctionExpression(BadWordToken.MakeWord(BadStaticKeys.CONSTRUCTOR_NAME),
                                       [],
                                       [new BadReturnExpression(new BadNullExpression(s_Position), s_Position, false)],
                                       s_Position,
                                       false,
                                       null,
                                       false,
                                       false,
                                       BadFunctionCompileLevel.None
                                      );
         BadPropertyDefinitionExpression propertyExpression =
             new BadPropertyDefinitionExpression(BadWordToken.MakeWord("Value"),
                                                 s_Position,
                                                 new BadNumberExpression(5, s_Position)
                                                );
         BadClassPrototypeExpression classExpression = new BadClassPrototypeExpression("TestClass",
                                                                                        [constructorExpression, propertyExpression],
                                                                                        [],
                                                                                        [],
                                                                                        s_Position,
                                                                                        null,
                                                                                        []
                                                                                       );
         BadInstruction[] instructions = BadCompiler.Compile([classExpression]).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadClassPrototype prototype = (BadClassPrototype)context.Scope.GetVariable("TestClass").Dereference(s_Position);
         BadClass instance = (BadClass)prototype.CreateInstance(context).Last();

         Assert.That(instance.TryGetMethodSlot(BadStaticKeys.CONSTRUCTOR_NAME, out _), Is.False);
         Assert.That(instance.TryGetMethodSlot("Value", out _), Is.False);
     }

     [Test]
     public void MethodSlot_MultipleInstancesAreIndependent()
     {
         const string source = """
                               class Counter
                               {
                                   let count = 0;
                                   compiled function Increment() { count = count + 1; }
                                   compiled function GetCount() { return count; }
                               }
                               let a = new Counter();
                               let b = new Counter();
                               a.Increment();
                               a.Increment();
                               b.Increment();
                               let countA = a.GetCount();
                               let countB = b.GetCount();
                               """;

         using BadRuntime runtime = new BadRuntime().UseCommonExtensions().UseCommonInterop();
         using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
         BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
         BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
         BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                true,
                                                                context.Scope,
                                                                s_Position,
                                                                BadWordToken.MakeWord("Test"),
                                                                false,
                                                                false,
                                                                null,
                                                                BadAnyPrototype.Instance,
                                                                false
                                                               );
         BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);

         foreach (BadObject _ in vm.Execute(context))
         {
         }

         BadObject countA = context.Scope.GetVariable("countA").Dereference(s_Position);
         BadObject countB = context.Scope.GetVariable("countB").Dereference(s_Position);

         Assert.That(((IBadNumber)countA).Value, Is.EqualTo(2));
         Assert.That(((IBadNumber)countB).Value, Is.EqualTo(1));
     }

    // ── AP4: Loop Fast-Path Tests ─────────────────────────────────────────────

    [Test]
    public void Foreach_OverNativeList_SumsElements()
    {
        string source = @"
let arr = [1, 2, 3];
let sum = 0;
foreach (x in arr) {
    sum += x;
}
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject sum = context.Scope.GetVariable("sum").Dereference(s_Position);
        Assert.That(((IBadNumber)sum).Value, Is.EqualTo(6));
    }

    [Test]
    public void Foreach_OverNativeList_BreakStopsIteration()
    {
        string source = @"
let arr = [10, 20, 30];
let count = 0;
foreach (x in arr) {
    count += 1;
    break;
}
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject count = context.Scope.GetVariable("count").Dereference(s_Position);
        Assert.That(((IBadNumber)count).Value, Is.EqualTo(1));
    }

    [Test]
    public void Foreach_NestedLoopsAreIndependent()
    {
        string source = @"
let outer = [1, 2];
let inner = [10, 20];
let sum = 0;
foreach (a in outer) {
    foreach (b in inner) {
        sum += b;
    }
}
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject sum = context.Scope.GetVariable("sum").Dereference(s_Position);
        // outer.Length * (10+20) = 2 * 30 = 60
        Assert.That(((IBadNumber)sum).Value, Is.EqualTo(60));
    }

    [Test]
    public void Foreach_EmptyList_BodyNeverExecuted()
    {
        string source = @"
let arr = [];
let count = 0;
foreach (x in arr) {
    count += 1;
}
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject count = context.Scope.GetVariable("count").Dereference(s_Position);
        Assert.That(((IBadNumber)count).Value, Is.EqualTo(0));
    }

    [Test]
    public void Foreach_BenchmarkPattern_KeepsScopeDepthBounded()
    {
        const int vmIterations = 1000;
        string source =
            "let acc = 0;\n" +
            "foreach(x in arr) { acc = acc + x; }\n" +
            "acc;\n";

        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        context.Scope.DefineVariable("arr",
                                     new BadArray(Enumerable.Range(0, vmIterations)
                                                            .Select(i => (BadObject)new BadNumber(i))
                                                            .ToList())
                                    );
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );

        ScopeDepthDebugger debugger = new ScopeDepthDebugger();
        BadDebugger.Attach(debugger);

        try
        {
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context))
            {
            }
        }
        finally
        {
            BadDebugger.Detach();
        }

        Assert.That(debugger.MaxDepth, Is.LessThan(20), $"Expected bounded scope depth, got {debugger.MaxDepth}");
    }

    [Test]
    public void Foreach_SingleLoop_KeepsScopeDepthBounded()
    {
        string arr = "[" + string.Join(", ", Enumerable.Range(0, 200)) + "]";
        string source =
            $"let arr = {arr};\n" +
            "let acc = 0;\n" +
            "foreach(x in arr) { acc = acc + x; }\n" +
            "acc;\n";

        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );

        ScopeDepthDebugger debugger = new ScopeDepthDebugger();
        BadDebugger.Attach(debugger);

        try
        {
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context))
            {
            }
        }
        finally
        {
            BadDebugger.Detach();
        }

        Assert.That(debugger.MaxDepth, Is.LessThan(20), $"Expected bounded scope depth, got {debugger.MaxDepth}");
    }

    [Test]
    public void While_SimpleBody_KeepsScopeDepthBounded()
    {
        string source =
            "let i = 0;\n" +
            "let acc = 0;\n" +
            "while(i < 200) {\n" +
            "    acc = acc + i;\n" +
            "    i = i + 1;\n" +
            "}\n" +
            "acc;\n";

        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );

        ScopeDepthDebugger debugger = new ScopeDepthDebugger();
        BadDebugger.Attach(debugger);

        try
        {
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context))
            {
            }
        }
        finally
        {
            BadDebugger.Detach();
        }

        Assert.That(debugger.MaxDepth, Is.LessThan(20), $"Expected bounded scope depth, got {debugger.MaxDepth}");
    }

    // ── AP5: LoadMember Property Cache Tests ─────────────────────────────────

    [Test]
    public void MemberCache_ReadFieldReturnsCorrectValue()
    {
        string source = @"
class Box {
    let Value = 99;
}
let b = new Box();
let v = b.Value;
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject v = context.Scope.GetVariable("v").Dereference(s_Position);
        Assert.That(((IBadNumber)v).Value, Is.EqualTo(99));
    }

    [Test]
    public void MemberCache_ReassignmentVisibleThroughCache()
    {
        string source = @"
class Counter {
    let Value = 0;
}
let c = new Counter();
c.Value = 42;
let v = c.Value;
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject v = context.Scope.GetVariable("v").Dereference(s_Position);
        Assert.That(((IBadNumber)v).Value, Is.EqualTo(42));
    }

    [Test]
    public void MemberCache_TwoInstancesHaveIndependentValues()
    {
        string source = @"
class Pair {
    let X = 0;
}
let a = new Pair();
let b = new Pair();
a.X = 100;
b.X = 200;
let va = a.X;
let vb = b.X;
";
        using BadRuntime runtime = new BadRuntime();
        runtime.UseCommonInterop();
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
        BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                               true,
                                                               context.Scope,
                                                               s_Position,
                                                               BadWordToken.MakeWord("Test"),
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
        foreach (BadObject _ in vm.Execute(context)) { }

        BadObject va = context.Scope.GetVariable("va").Dereference(s_Position);
        BadObject vb = context.Scope.GetVariable("vb").Dereference(s_Position);
        Assert.That(((IBadNumber)va).Value, Is.EqualTo(100));
        Assert.That(((IBadNumber)vb).Value, Is.EqualTo(200));
    }

    // ── Settings Rollout Tests ────────────────────────────────────────────────

    [Test]
    public void Settings_UsePropertyReferenceCache_ToggleFalse_StillProducesCorrectResult()
    {
        bool prev = BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache;
        try
        {
            BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache = false;
            const string source = @"
class Box { let num Value = 7; }
let b = new Box();
let r = b.Value;
";
            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions, true, context.Scope, s_Position,
                BadWordToken.MakeWord("Test"), false, false, null, BadAnyPrototype.Instance, false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }
            BadObject r = context.Scope.GetVariable("r").Dereference(s_Position);
            Assert.That(((IBadNumber)r).Value, Is.EqualTo(7));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache = prev;
        }
    }

    [Test]
    public void Settings_UseLoopFastPath_ToggleFalse_StillProducesCorrectResult()
    {
        bool prev = BadNativeOptimizationSettings.Instance.UseLoopFastPath;
        try
        {
            BadNativeOptimizationSettings.Instance.UseLoopFastPath = false;
            const string source = @"
let arr = [10, 20, 30];
let sum = 0;
foreach(x in arr) { sum = sum + x; }
";
            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions, true, context.Scope, s_Position,
                BadWordToken.MakeWord("Test"), false, false, null, BadAnyPrototype.Instance, false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }
            BadObject sum = context.Scope.GetVariable("sum").Dereference(s_Position);
            Assert.That(((IBadNumber)sum).Value, Is.EqualTo(60));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseLoopFastPath = prev;
        }
    }

    [Test]
    public void Settings_UseMethodSlotFastPath_ToggleFalse_StillProducesCorrectResult()
    {
        bool prev = BadNativeOptimizationSettings.Instance.UseMethodSlotFastPath;
        try
        {
            BadNativeOptimizationSettings.Instance.UseMethodSlotFastPath = false;
            const string source = @"
class Adder {
    function num Add(num a, num b) { return a + b; }
}
let adder = new Adder();
let r = adder.Add(3, 4);
";
            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions, true, context.Scope, s_Position,
                BadWordToken.MakeWord("Test"), false, false, null, BadAnyPrototype.Instance, false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }
            BadObject r = context.Scope.GetVariable("r").Dereference(s_Position);
            Assert.That(((IBadNumber)r).Value, Is.EqualTo(7));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseMethodSlotFastPath = prev;
        }
    }

    // ── VM Burst Mode Tests ──────────────────────────────────────────────────

    [Test]
    public void VmBurst_BurstSize5_ProducesSameResultAsSize1()
    {
        int prev = BadNativeOptimizationSettings.Instance.VmBurstSize;
        try
        {
            const string source = @"
let acc = 0;
let i = 0;
while (i < 20) {
    acc = acc + i;
    i = i + 1;
}
";
            BadNativeOptimizationSettings.Instance.VmBurstSize = 1;
            using BadRuntime runtime1 = new BadRuntime();
            runtime1.UseCommonInterop();
            using BadExecutionContext ctx1 = runtime1.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] exprs = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instrs = BadCompiler.Compile(exprs).ToArray();
            BadCompiledFunction fn1 = new BadCompiledFunction(instrs, true, ctx1.Scope, s_Position,
                BadWordToken.MakeWord("Test"), false, false, null, BadAnyPrototype.Instance, false);
            BadRuntimeVirtualMachine vm1 = new BadRuntimeVirtualMachine(fn1, instrs);
            foreach (BadObject _ in vm1.Execute(ctx1)) { }
            decimal acc1 = ((IBadNumber)ctx1.Scope.GetVariable("acc").Dereference(s_Position)).Value;

            BadNativeOptimizationSettings.Instance.VmBurstSize = 5;
            using BadRuntime runtime5 = new BadRuntime();
            runtime5.UseCommonInterop();
            using BadExecutionContext ctx5 = runtime5.CreateContext(Directory.GetCurrentDirectory());
            BadInstruction[] instrs5 = BadCompiler.Compile(BadRuntime.Parse(source, "<test>").ToArray()).ToArray();
            BadCompiledFunction fn5 = new BadCompiledFunction(instrs5, true, ctx5.Scope, s_Position,
                BadWordToken.MakeWord("Test"), false, false, null, BadAnyPrototype.Instance, false);
            BadRuntimeVirtualMachine vm5 = new BadRuntimeVirtualMachine(fn5, instrs5);
            foreach (BadObject _ in vm5.Execute(ctx5)) { }
            decimal acc5 = ((IBadNumber)ctx5.Scope.GetVariable("acc").Dereference(s_Position)).Value;

            Assert.That(acc5, Is.EqualTo(acc1));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.VmBurstSize = prev;
        }
    }

    [Test]
    public void VmBurst_BurstSize1Clamp_RejectsZero()
    {
        int prev = BadNativeOptimizationSettings.Instance.VmBurstSize;
        try
        {
            BadNativeOptimizationSettings.Instance.VmBurstSize = 0;
            Assert.That(BadNativeOptimizationSettings.Instance.VmBurstSize, Is.EqualTo(1));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.VmBurstSize = prev;
        }
    }

    [Test]
    public void VmBurst_WithEarlyReturn_ReturnsCorrectly()
    {
        int prev = BadNativeOptimizationSettings.Instance.VmBurstSize;
        try
        {
            BadNativeOptimizationSettings.Instance.VmBurstSize = 4;
            const string source = @"
function num First(arr) {
    foreach(x in arr) { return x; }
    return -1;
}
let r = First([42, 99, 0]);
";
            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions, true, context.Scope, s_Position,
                BadWordToken.MakeWord("Test"), false, false, null, BadAnyPrototype.Instance, false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }
            BadObject r = context.Scope.GetVariable("r").Dereference(s_Position);
            Assert.That(((IBadNumber)r).Value, Is.EqualTo(42));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.VmBurstSize = prev;
        }
    }

    [Test]
    public void PhaseA_BinaryAndComparisonSpecialization_RecordsFastPathHits()
    {
        bool prevBinary = BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization;
        bool prevComparison = BadNativeOptimizationSettings.Instance.UseComparisonSpecialization;
        bool prevLoopCondition = BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization;

        try
        {
            BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization = true;
            BadNativeOptimizationSettings.Instance.UseComparisonSpecialization = true;
            BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization = true;
            BadRuntimeVirtualMachine.ResetOptimizationCounters();

            const string source = @"
let sum = 0;
let i = 0;
while (i < 20) {
    sum = sum + i;
    i = i + 1;
}
";

            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                   true,
                                                                   context.Scope,
                                                                   s_Position,
                                                                   BadWordToken.MakeWord("Test"),
                                                                   false,
                                                                   false,
                                                                   null,
                                                                   BadAnyPrototype.Instance,
                                                                   false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }

            decimal sum = ((IBadNumber)context.Scope.GetVariable("sum").Dereference(s_Position)).Value;
            Dictionary<string, long> counters = BadRuntimeVirtualMachine.GetOptimizationCounterSnapshot();

            Assert.That(sum, Is.EqualTo(190));
            Assert.That(counters["BinaryOperatorSpecializationHit"], Is.GreaterThan(0));
            Assert.That(counters["ComparisonSpecializationHit"], Is.GreaterThan(0));
            Assert.That(counters["LoopConditionSpecialization"], Is.GreaterThan(0));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization = prevBinary;
            BadNativeOptimizationSettings.Instance.UseComparisonSpecialization = prevComparison;
            BadNativeOptimizationSettings.Instance.UseLoopConditionSpecialization = prevLoopCondition;
        }
    }

    [Test]
    public void PhaseB_LoadMemberInlineCache_RecordsCacheHits()
    {
        bool prevInlineCaching = BadNativeOptimizationSettings.Instance.UseInlineCaching;
        bool prevPropertyReferenceCache = BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache;

        try
        {
            BadNativeOptimizationSettings.Instance.UseInlineCaching = true;
            BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache = false;
            BadRuntimeVirtualMachine.ResetOptimizationCounters();

            const string source = @"
class Box {
    let num Value = 1;
}

let b = new Box();
let acc = 0;
let i = 0;
while (i < 20) {
    acc = acc + b.Value;
    i = i + 1;
}
";

            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                   true,
                                                                   context.Scope,
                                                                   s_Position,
                                                                   BadWordToken.MakeWord("Test"),
                                                                   false,
                                                                   false,
                                                                   null,
                                                                   BadAnyPrototype.Instance,
                                                                   false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }

            decimal acc = ((IBadNumber)context.Scope.GetVariable("acc").Dereference(s_Position)).Value;
            Dictionary<string, long> counters = BadRuntimeVirtualMachine.GetOptimizationCounterSnapshot();

            Assert.That(acc, Is.EqualTo(20));
            Assert.That(counters["LoadMemberInlineCacheHit"], Is.GreaterThan(0));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseInlineCaching = prevInlineCaching;
            BadNativeOptimizationSettings.Instance.UsePropertyReferenceCache = prevPropertyReferenceCache;
        }
    }

    [Test]
    public void PhaseB_NullCheckInlineCache_RecordsCacheHits()
    {
        bool prevInlineCaching = BadNativeOptimizationSettings.Instance.UseInlineCaching;
        bool prevNullCheckInlineCache = BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache;

        try
        {
            BadNativeOptimizationSettings.Instance.UseInlineCaching = true;
            BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache = true;
            BadRuntimeVirtualMachine.ResetOptimizationCounters();

            const string source = @"
class Box {
    let num Value = 2;
}

let b = new Box();
let last = null;
let i = 0;
while (i < 20) {
    last = b?.Value;
    i = i + 1;
}
";

            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                   true,
                                                                   context.Scope,
                                                                   s_Position,
                                                                   BadWordToken.MakeWord("Test"),
                                                                   false,
                                                                   false,
                                                                   null,
                                                                   BadAnyPrototype.Instance,
                                                                   false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }

            decimal last = ((IBadNumber)context.Scope.GetVariable("last").Dereference(s_Position)).Value;
            Dictionary<string, long> counters = BadRuntimeVirtualMachine.GetOptimizationCounterSnapshot();

            Assert.That(last, Is.EqualTo(2));
            Assert.That(counters["NullCheckInlineCacheHit"], Is.GreaterThan(0));
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseInlineCaching = prevInlineCaching;
            BadNativeOptimizationSettings.Instance.UseNullCheckInlineCache = prevNullCheckInlineCache;
        }
    }

    [Test]
    public void PhaseC1_IntegerCache_ReturnsCorrectResults()
    {
        // Verify BadNumber.Get() returns cached singletons for integers and correct values.
        BadNumber n0a = BadNumber.Get(0);
        BadNumber n0b = BadNumber.Get(0);
        BadNumber n1 = BadNumber.Get(1);
        BadNumber nNeg = BadNumber.Get(-1);
        BadNumber nLarge = BadNumber.Get(99999m); // beyond cache range → new instance

        Assert.That(((IBadNumber)n0a).Value, Is.EqualTo(0m));
        Assert.That(ReferenceEquals(n0a, n0b), Is.True, "Integer cache should return the same instance");
        Assert.That(ReferenceEquals(n0a, n1), Is.False);
        Assert.That(((IBadNumber)nNeg).Value, Is.EqualTo(-1m));
        Assert.That(((IBadNumber)nLarge).Value, Is.EqualTo(99999m));
        // Non-integer should NOT be cached
        BadNumber nFrac1 = BadNumber.Get(1.5m);
        BadNumber nFrac2 = BadNumber.Get(1.5m);
        Assert.That(ReferenceEquals(nFrac1, nFrac2), Is.False, "Fractional values are not cached");
    }

    [Test]
    public void PhaseC2_EscapeAnalysis_TransientScratch_RecordsScratchHits()
    {
        bool prevEscape = BadNativeOptimizationSettings.Instance.UseEscapeAnalysis;
        bool prevBinarySpec = BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization;

        try
        {
            BadNativeOptimizationSettings.Instance.UseEscapeAnalysis = true;
            BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization = true;
            BadRuntimeVirtualMachine.ResetOptimizationCounters();

            // sum = sum + (i * 3) + (i * 2) in a loop
            // Instruction sequence for RHS:
            //   LoadLocal sum, LoadLocal i, Push 3, Mul, Add, LoadLocal i, Push 2, Mul(*), Add
            //   (*) Mul is immediately followed by Add → TransientResult → scratch reused
            const string source = @"
function Main() {
    let sum = 0;
    let i = 0;
    while (i < 10) {
        sum = sum + (i * 3) + (i * 2);
        i = i + 1;
    }
    return sum;
}
Main();
";

            using BadRuntime runtime = new BadRuntime();
            runtime.UseCommonInterop();
            using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
            BadExpression[] expressions = BadRuntime.Parse(source, "<test>").ToArray();
            BadInstruction[] instructions = BadCompiler.Compile(expressions).ToArray();
            BadCompiledFunction function = new BadCompiledFunction(instructions,
                                                                   true,
                                                                   context.Scope,
                                                                   s_Position,
                                                                   BadWordToken.MakeWord("Test"),
                                                                   false,
                                                                   false,
                                                                   null,
                                                                   BadAnyPrototype.Instance,
                                                                   false);
            BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(function, instructions);
            foreach (BadObject _ in vm.Execute(context)) { }

            Dictionary<string, long> counters = BadRuntimeVirtualMachine.GetOptimizationCounterSnapshot();

            // 2 transient Mul ops per iteration × 10 iterations = 20 scratch hits
            Assert.That(counters["EscapeAnalysisScratchHit"], Is.GreaterThanOrEqualTo(10),
                        "Expected scratch hits from transient Mul results");
        }
        finally
        {
            BadNativeOptimizationSettings.Instance.UseEscapeAnalysis = prevEscape;
            BadNativeOptimizationSettings.Instance.UseBinaryOperatorSpecialization = prevBinarySpec;
        }
    }

    private sealed class ScopeDepthDebugger : IBadDebugger
    {
        public int MaxDepth { get; private set; }

        public void Step(BadDebuggerStep stepInfo)
        {
            MaxDepth = Math.Max(MaxDepth, stepInfo.Context.Scope.Depth);
        }
    }
}
