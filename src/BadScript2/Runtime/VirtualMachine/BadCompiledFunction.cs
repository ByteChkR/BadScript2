using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Virtual Machine Implementation.
/// </summary>
namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Implements a Compiled Function.
/// </summary>
public class BadCompiledFunction : BadFunction
{
    /// <summary>
    ///     The Compiled Function's Instructions.
    /// </summary>
    private readonly BadInstruction[] m_Instructions;

    /// <summary>
    ///     The Compiled Function's Parent Scope.
    /// </summary>
    private readonly BadScope m_ParentScope;

    /// <summary>
    ///     The Original Source Position.
    /// </summary>
    private readonly BadSourcePosition m_Position;

    /// <summary>
    ///     The Signature
    /// </summary>
    private readonly string m_StringSignature;

    /// <summary>
    ///     Indicates if the Function should use Operator Overrides.
    /// </summary>
    private readonly bool m_UseOverrides;

    /// <summary>
    ///     Creates a new <see cref="BadCompiledFunction" /> instance.
    /// </summary>
    /// <param name="instructions">The list of Instructions.</param>
    /// <param name="useOverrides">Indicates if the Function should use Operator Overrides.</param>
    /// <param name="parentScope">The Parent Scope.</param>
    /// <param name="position">The Original Source Position.</param>
    /// <param name="name">The Function's Name.</param>
    /// <param name="isConstant">Indicates if the Function is Constant.</param>
    /// <param name="isStatic">Indicates if the Function is Static.</param>
    /// <param name="metaData">The Function's MetaData.</param>
    /// <param name="returnType">The Function's Return Type.</param>
    /// <param name="isSingleLine">Indicates if the function is a single line function</param>
    /// <param name="parameters">The Function's Parameters.</param>
    public BadCompiledFunction(
        BadInstruction[] instructions,
        bool useOverrides,
        BadScope parentScope,
        BadSourcePosition position,
        BadWordToken? name,
        bool isConstant,
        bool isStatic,
        BadMetaData? metaData,
        BadClassPrototype returnType,
        bool isSingleLine,
        params BadFunctionParameter[] parameters) : base(
        name,
        isConstant,
        isStatic,
        returnType,
        isSingleLine,
        parameters
    )
    {
        m_Instructions = instructions;
        m_UseOverrides = useOverrides;
        m_ParentScope = parentScope;
        m_Position = position;
        MetaData = metaData ?? BadMetaData.Empty;
        m_StringSignature = MakeSignature();
    }

    /// <inheritdoc />
    public override BadMetaData MetaData { get; }

    public BadExecutionContext CreateExecutionContext(BadExecutionContext caller, BadObject[] args)
    {
        BadExecutionContext ctx = new BadExecutionContext(
            m_ParentScope.CreateChild(
                "Compiled Function",
                caller.Scope,
                null,
                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
            )
        );
        ApplyParameters(ctx, args, m_Position);
        return ctx;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        using BadExecutionContext ctx = CreateExecutionContext(caller, args);

        if (m_Instructions.Length == 0)
        {
            yield return Null;

            yield break;
        }

        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(this, m_Instructions, m_UseOverrides);

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
    }

    /// <summary>
    ///     Creates a Signature for this Function.
    /// </summary>
    /// <returns>The Signature.</returns>
    private string MakeSignature()
    {
        string str = base.ToString() + " at " + m_Position.GetPositionInfo() + '\n';

        for (int i = 0; i < m_Instructions.Length; i++)
        {
            str += i + ":\t" + m_Instructions[i] + '\n';
        }

        return str;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return m_StringSignature;
    }
}