using BadScript2.Parser;
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
    /// <summary>
    /// The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, BadObject[], BadObject> m_Func;

    /// <summary>
    /// Contains meta data for this function
    /// </summary>
    private BadMetaData? _metaData;
    /// <inheritdoc/>
    public override BadMetaData MetaData => _metaData ?? BadMetaData.Empty;

    /// <summary>
    /// Creates a new BadInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="isStatic">Indicates if the Function is Static</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadInteropFunction(
        BadWordToken? name,
        Func<BadObject[], BadObject> func,
        bool isStatic,
        BadClassPrototype returnType,
        params BadFunctionParameter[] parameters) : base(name, false, isStatic, returnType, parameters)
    {
        m_Func = (_, args) => func(args);
    }
    
    /// <summary>
    /// Sets the MetaData for this Function
    /// </summary>
    /// <param name="metaData">The MetaData to set</param>
    /// <returns>This Function</returns>
    public BadInteropFunction SetMetaData(BadMetaData metaData)
    {
        _metaData = metaData;
        return this;
    }
    

    /// <summary>
    /// Creates a new BadInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="isStatic">Indicates if the Function is Static</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, BadObject[], BadObject> func,
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
    public static BadInteropFunction Create(
        Func<BadObject[], BadObject> func,
        bool isStatic,
        BadClassPrototype returnType,
        params string[] names)
    {
        BadInteropFunction function = new BadInteropFunction(
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

        yield return m_Func.Invoke(caller, args);
    }
}