using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Functions;

/// <summary>
///     Interop Function taking an array of arguments
/// </summary>
public class BadEnumerableInteropFunction : BadFunction
{
    /// <summary>
    /// The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, BadObject[], IEnumerable<BadObject>> m_Func;

    /// <summary>
    /// Creates a new BadInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="isStatic">Indicates if the Function is Static</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadEnumerableInteropFunction(
        BadWordToken? name,
        Func<BadObject[], IEnumerable<BadObject>> func,
        bool isStatic,
        BadClassPrototype returnType,
        params BadFunctionParameter[] parameters) : base(name, false, isStatic, returnType, parameters)
    {
        m_Func = (_, args) => func(args);
    }

    /// <summary>
    /// Creates a new BadInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="isStatic">Indicates if the Function is Static</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadEnumerableInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, BadObject[], IEnumerable<BadObject>> func,
        bool isStatic,
        BadClassPrototype returnType,
        params BadFunctionParameter[] parameters) : base(name, false, isStatic, returnType, parameters)
    {
        m_Func = func;
    }


    /// <summary>
    /// Creates a new BadInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <param name="isStatic">Indicates if the Function is Static</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="names">The Parameters of the Function</param>
    public static BadEnumerableInteropFunction Create(
        Func<BadObject[], IEnumerable<BadObject>> func,
        bool isStatic,
        BadClassPrototype returnType,
        params string[] names)
    {
        BadEnumerableInteropFunction function = new BadEnumerableInteropFunction(
            null,
            func,
            isStatic,
            returnType,
            names.Select(x => (BadFunctionParameter)x).ToArray()
        );

        return function;
    }


    /// <inheritdoc/>
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        return m_Func.Invoke(caller, args);
    }
}