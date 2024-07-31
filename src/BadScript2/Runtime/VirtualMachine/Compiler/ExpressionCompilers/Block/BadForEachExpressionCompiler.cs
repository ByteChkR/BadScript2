using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains Block Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadForEachExpression" />.
/// </summary>
public class BadForEachExpressionCompiler : BadExpressionCompiler<BadForEachExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadForEachExpression expression)
    {
        context.Compile(expression.Target);
        context.Emit(BadOpCode.Dup, expression.Position);
        context.Emit(BadOpCode.Push, expression.Position, (BadObject)"GetEnumerator");
        context.Emit(BadOpCode.HasProperty, expression.Position);
        context.Emit(BadOpCode.JumpRelativeIfFalse, expression.Position, 2);
        context.Emit(BadOpCode.LoadMember, expression.Position, "GetEnumerator");
        context.Emit(BadOpCode.Invoke, expression.Position, 0);
        context.Emit(BadOpCode.CreateScope, expression.Position, "FOREACH_ENUMERATOR_SCOPE", BadObject.Null);
        context.Emit(BadOpCode.DefVar, expression.Position, "~ENUMERATOR~", true);
        context.Emit(BadOpCode.Swap, expression.Position);
        context.Emit(BadOpCode.Assign, expression.Position);
        List<BadExpression>? body = expression.Body.ToList();

        body.Insert(0,
                    new BadAssignExpression(new BadVariableDefinitionExpression(expression.LoopVariable.Text,
                                                                                expression.LoopVariable.SourcePosition,
                                                                                null,
                                                                                true
                                                                               ),
                                            new BadInvocationExpression(new
                                                                            BadMemberAccessExpression(new
                                                                                     BadVariableExpression("~ENUMERATOR~",
                                                                                          expression.Position
                                                                                         ),
                                                                                 "GetCurrent",
                                                                                 expression.Position,
                                                                                 new List<BadExpression>()
                                                                                ),
                                                                        Array.Empty<BadExpression>(),
                                                                        expression.Position
                                                                       ),
                                            expression.Position
                                           )
                   );

        context.Compile(new BadWhileExpression(new BadInvocationExpression(new
                                                                               BadMemberAccessExpression(new
                                                                                        BadVariableExpression("~ENUMERATOR~",
                                                                                             expression.LoopVariable
                                                                                                 .SourcePosition
                                                                                            ),
                                                                                    "MoveNext",
                                                                                    expression.LoopVariable
                                                                                        .SourcePosition,
                                                                                    new List<BadExpression>()
                                                                                   ),
                                                                           Array.Empty<BadExpression>(),
                                                                           expression.Position
                                                                          ),
                                               body,
                                               expression.Position
                                              )
                       );
        context.Emit(BadOpCode.DestroyScope, expression.Position);
    }
}