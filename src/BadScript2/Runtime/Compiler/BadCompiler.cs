using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Compiler.Expression;
using BadScript2.Runtime.Compiler.Expression.Access;
using BadScript2.Runtime.Compiler.Expression.Binary;
using BadScript2.Runtime.Compiler.Expression.Binary.Comparison;
using BadScript2.Runtime.Compiler.Expression.Binary.Logic;
using BadScript2.Runtime.Compiler.Expression.Binary.Logic.Assign;
using BadScript2.Runtime.Compiler.Expression.Binary.Math;
using BadScript2.Runtime.Compiler.Expression.Binary.Math.Assign;
using BadScript2.Runtime.Compiler.Expression.Binary.Math.Atomic;
using BadScript2.Runtime.Compiler.Expression.Block;
using BadScript2.Runtime.Compiler.Expression.Constant;
using BadScript2.Runtime.Compiler.Expression.ControlFlow;
using BadScript2.Runtime.Compiler.Expression.Function;
using BadScript2.Runtime.Compiler.Expression.Types;
using BadScript2.Runtime.Compiler.Expression.Variables;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.Compiler
{
    public static class BadCompiler
    {
        private static readonly List<IBadExpressionCompiler> s_Compilers = new List<IBadExpressionCompiler>
        {
            new BadPostIncrementExpressionCompiler(),
            new BadPreIncrementExpressionCompiler(),
            new BadPostDecrementExpressionCompiler(),
            new BadPreDecrementExpressionCompiler(),

            new BadAddAssignExpressionCompiler(),
            new BadSubtractExpressionCompiler(),
            new BadMultiplyExpressionCompiler(),
            new BadDivideExpressionCompiler(),
            new BadModulusExpressionCompiler(),

            new BadAddExpressionCompiler(),
            new BadSubtractExpressionCompiler(),
            new BadMultiplyExpressionCompiler(),
            new BadDivideExpressionCompiler(),
            new BadModulusExpressionCompiler(),

            new BadNumberExpressionCompiler(),
            new BadStringExpressionCompiler(),
            new BadBooleanExpressionCompiler(),
            new BadNullExpressionCompiler(),
            new BadArrayExpressionCompiler(),
            new BadTableExpressionCompiler(),

            new BadVariableDefinitionExpressionCompiler(),
            new BadVariableExpressionCompiler(),

            new BadReturnExpressionCompiler(),
            new BadThrowExpressionCompiler(),
            new BadAssignExpressionCompiler(),

            new BadWhileExpressionCompiler(),
            new BadForExpressionCompiler(),
            new BadIfExpressionCompiler(),

            new BadLessThanExpressionCompiler(),
            new BadLessOrEqualExpressionCompiler(),
            new BadGreaterThanExpressionCompiler(),
            new BadGreaterOrEqualExpressionCompiler(),
            new BadEqualExpressionCompiler(),
            new BadNotEqualExpressionCompiler(),

            new BadLogicAssignAndExpressionCompiler(),
            new BadLogicAssignOrExpressionCompiler(),
            new BadLogicAssignXOrExpressionCompiler(),

            new BadLogicAndExpressionCompiler(),
            new BadLogicOrExpressionCompiler(),
            new BadLogicNotExpressionCompiler(),
            new BadLogicXOrExpressionCompiler(),

            new BadMemberAccessExpressionCompiler(),
            new BadArrayAccessExpressionCompiler(),

            new BadInvocationExpressionCompiler(),
            new BadFunctionExpressionCompiler(),
            new BadNewExpressionCompiler(),
            new BadNullCoalescingExpressionCompiler(),
            new BadNullCoalescingAssignExpressionCompiler(),
            new BadTernaryExpressionCompiler(),
        };

        public static int CompileExpression(BadExpression expr, BadCompilerResult result)
        {
            IBadExpressionCompiler? compiler = s_Compilers.FirstOrDefault(x => x.CanCompile(expr));
            if (compiler == null)
            {
                throw new BadRuntimeException($"Cannot compile expression '{expr.GetType().Name}'", expr.Position);
            }

            return compiler.Compile(expr, result);
        }

        public static void CompileFunction(
            BadFunctionExpression func,
            out int funcStart,
            out int funcLen,
            BadCompilerResult result)
        {
            if (func.Name != null)
            {
                funcStart = result.DefineLabel(func.Name.Text);
            }
            else
            {
                funcStart = result.DefineLabel(result.UniqueLabel("func_"));
            }

            foreach (BadExpression expression in func.Body)
            {
                CompileExpression(expression, result);
                result.Emit(new BadInstruction(BadOpCode.ClearStack, BadSourcePosition.FromSource("", 0, 0)));
            }


            result.Emit(new BadInstruction(BadOpCode.Push, BadSourcePosition.FromSource("", 0, 0), BadObject.Null));

            result.Emit(new BadInstruction(BadOpCode.Return, BadSourcePosition.FromSource("", 0, 0)));

            funcLen = result.GetInstructionLength() - funcStart;
        }


        private static void CompileFunctionBlock(
            IEnumerable<BadExpression> block,
            out int funcStart,
            out int funcLen,
            BadCompilerResult result,
            string? name = null)
        {
            if (name != null)
            {
                funcStart = result.DefineLabel(name);
            }
            else
            {
                funcStart = result.DefineLabel(result.UniqueLabel("func_"));
            }

            foreach (BadExpression expression in block)
            {
                CompileExpression(expression, result);
                result.Emit(new BadInstruction(BadOpCode.ClearStack, BadSourcePosition.FromSource("", 0, 0)));
            }


            result.Emit(new BadInstruction(BadOpCode.Push, BadSourcePosition.FromSource("", 0, 0), BadObject.Null));

            result.Emit(new BadInstruction(BadOpCode.Return, BadSourcePosition.FromSource("", 0, 0)));

            funcLen = result.GetInstructionLength() - funcStart;
        }

        private static void CompileFunction(
            BadExpressionFunction func,
            out int funcStart,
            out int funcLen,
            BadCompilerResult result)
        {
            CompileFunctionBlock(func.Body, out funcStart, out funcLen, result, func.Name?.Text);
        }


        public static BadCompiledFunction Compile(IEnumerable<BadExpression> block, BadScope parentScope)
        {
            BadCompilerResult result = new BadCompilerResult(parentScope);
            CompileFunctionBlock(block, out int funcStart, out int funcLen, result);
            result.ProcessWorkItems();

            BadLogger.Log($"Compiled {result.GetInstructions().Length} Instructions", "Runtime");

            return new BadCompiledFunction(
                null,
                parentScope,
                BadSourcePosition.FromSource("", 0, 0),
                result,
                funcStart,
                funcLen
            );
        }

        public static BadFunction Compile(BadExpressionFunction func)
        {
            BadLogger.Log($"Compiling {func}", "Runtime");
            BadCompilerResult result = new BadCompilerResult(func.ParentScope);
            CompileFunction(func, out int start, out int len, result);
            result.ProcessWorkItems();
            BadCompiledFunction f = new BadCompiledFunction(
                func.Name,
                func.ParentScope,
                func.Position,
                result,
                start,
                len,
                func.Parameters
            );


            BadLogger.Log($"Compiled {result.GetInstructions().Length} Instructions", "Runtime");

            return f;
        }
    }
}