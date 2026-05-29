using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.VirtualMachine;

namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Structured representation of a property accessor used by compiled class templates.
/// </summary>
public sealed class BadCompiledPropertyAccessorTemplate
{
    private readonly AccessorFastPathKind m_FastPathKind;

    private readonly string? m_FastPathVariableName;

    private readonly string? m_FastPathMemberName;

    private readonly BadInstruction[]? m_CompiledInstructions;

    private readonly bool m_UseOverrides;

    public BadCompiledPropertyAccessorTemplate(BadExpression expression,
                                               bool forceCompile = false,
                                               bool forceUseOverrides = true)
    {
        Expression = expression;
        (m_FastPathKind, m_FastPathVariableName, m_FastPathMemberName) = AnalyzeFastPath(expression);
        m_CompiledInstructions = TryCompileAccessor(expression, forceCompile, forceUseOverrides, out m_UseOverrides);
    }

    /// <summary>
    /// Original accessor expression.
    /// </summary>
    public BadExpression Expression { get; }

    /// <summary>
    /// Optional precompiled instructions for simple accessor expressions.
    /// </summary>
    public IReadOnlyList<BadInstruction>? CompiledInstructions => m_CompiledInstructions;

    /// <summary>
    /// Indicates whether compiled accessor execution should use operator overrides.
    /// </summary>
    public bool UseOverrides => m_UseOverrides;

    /// <summary>
    /// Executes the accessor expression in the provided context.
    /// </summary>
    public IEnumerable<BadObject> Execute(BadExecutionContext context)
    {
        if (m_FastPathKind == AccessorFastPathKind.VariableRead)
        {
            return ExecuteDirectVariableRead(context);
        }

        if (m_FastPathKind == AccessorFastPathKind.AssignFromValueVariable)
        {
            return ExecuteDirectAssignFromValueVariable(context);
        }

        if (m_FastPathKind == AccessorFastPathKind.ThisMemberRead)
        {
            return ExecuteDirectThisMemberRead(context);
        }

        if (m_FastPathKind == AccessorFastPathKind.ThisMemberAssignFromValue)
        {
            return ExecuteDirectThisMemberAssignFromValue(context);
        }

        if (m_CompiledInstructions != null)
        {
            return ExecuteCompiled(context);
        }

        return Expression.Execute(context);
    }

    private IEnumerable<BadObject> ExecuteCompiled(BadExecutionContext context)
    {
        BadScope executionScope = context.Scope.CreateChild("Compiled Property Accessor",
                                                            context.Scope,
                                                            null,
                                                            BadScopeFlags.Returnable |
                                                            BadScopeFlags.AllowThrow |
                                                            BadScopeFlags.CaptureThrow
                                                           );
        using BadExecutionContext executionContext = new BadExecutionContext(executionScope);
        BadCompiledFunction accessor = new BadCompiledFunction(m_CompiledInstructions!,
                                                               m_UseOverrides,
                                                               executionScope,
                                                               Expression.Position,
                                                               null,
                                                               false,
                                                               false,
                                                               null,
                                                               BadAnyPrototype.Instance,
                                                               false
                                                              );
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(accessor, m_CompiledInstructions!, m_UseOverrides);

        foreach (BadObject o in vm.Execute(executionContext))
        {
            yield return o;
        }

        yield return executionScope.ReturnValue ?? BadObject.Null;
    }

    private IEnumerable<BadObject> ExecuteDirectVariableRead(BadExecutionContext context)
    {
        BadObjectReference variable = context.Scope.GetVariable(m_FastPathVariableName!, context.Scope);

        yield return variable;
    }

    private IEnumerable<BadObject> ExecuteDirectAssignFromValueVariable(BadExecutionContext context)
    {
        BadObjectReference target = context.Scope.GetVariable(m_FastPathVariableName!, context.Scope);
        BadObject value = context.Scope.GetVariable("value", context.Scope)
                                  .Dereference(Expression.Position);

        target.Set(value, Expression.Position);

        yield return target;
    }

    private IEnumerable<BadObject> ExecuteDirectThisMemberRead(BadExecutionContext context)
    {
        BadObject thisObject = context.Scope.GetVariable(BadStaticKeys.THIS_KEY, context.Scope)
                                     .Dereference(Expression.Position);

        yield return thisObject.GetProperty(m_FastPathMemberName!, context.Scope);
    }

    private IEnumerable<BadObject> ExecuteDirectThisMemberAssignFromValue(BadExecutionContext context)
    {
        BadObject thisObject = context.Scope.GetVariable(BadStaticKeys.THIS_KEY, context.Scope)
                                     .Dereference(Expression.Position);
        BadObjectReference target = thisObject.GetProperty(m_FastPathMemberName!, context.Scope);
        BadObject value = context.Scope.GetVariable("value", context.Scope)
                                  .Dereference(Expression.Position);

        target.Set(value, Expression.Position);

        yield return target;
    }

    private static (AccessorFastPathKind Kind, string? VariableName, string? MemberName) AnalyzeFastPath(BadExpression expression)
    {
        if (expression is BadVariableExpression variableExpression)
        {
            return (AccessorFastPathKind.VariableRead, variableExpression.Name, null);
        }

        if (expression is BadMemberAccessExpression
            {
                Left: BadVariableExpression { Name: BadStaticKeys.THIS_KEY },
                NullChecked: false,
                GenericArguments: { Count: 0 },
            } memberAccessExpression)
        {
            return (AccessorFastPathKind.ThisMemberRead, null, memberAccessExpression.Right.Text);
        }

        if (expression is BadAssignExpression
            {
                Left: BadVariableExpression left,
                Right: BadVariableExpression { Name: "value" },
            })
        {
            return (AccessorFastPathKind.AssignFromValueVariable, left.Name, null);
        }

        if (expression is BadAssignExpression
            {
                Left: BadMemberAccessExpression
                {
                    Left: BadVariableExpression { Name: BadStaticKeys.THIS_KEY },
                    NullChecked: false,
                    GenericArguments: { Count: 0 },
                } leftMember,
                Right: BadVariableExpression { Name: "value" },
            })
        {
            return (AccessorFastPathKind.ThisMemberAssignFromValue, null, leftMember.Right.Text);
        }

        return (AccessorFastPathKind.None, null, null);
    }

    private static BadInstruction[]? TryCompileAccessor(BadExpression expression,
                                                        bool forceCompile,
                                                        bool forceUseOverrides,
                                                        out bool useOverrides)
    {
        useOverrides = forceUseOverrides;

        if (!forceCompile && !IsSimpleAccessorExpression(expression))
        {
            return null;
        }

        try
        {
            BadReturnExpression returnExpression = new BadReturnExpression(expression, expression.Position, false);

            return BadCompiler.Compile(new BadExpression[] { returnExpression }).ToArray();
        }
        catch
        {
            return null;
        }
    }

    private static bool IsSimpleAccessorExpression(BadExpression expression)
    {
        return expression is BadVariableExpression or
            BadMemberAccessExpression or
            BadNumberExpression or
            BadStringExpression or
            BadBooleanExpression or
            BadNullExpression;
    }

    private enum AccessorFastPathKind
    {
        None,
        VariableRead,
        AssignFromValueVariable,
        ThisMemberRead,
        ThisMemberAssignFromValue,
    }
}

