using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Interop Function Classes for the BadScript2 Language
/// </summary>
namespace BadScript2.Runtime.Interop.Functions;

/// <summary>
///     Non Generic Interop Function.
/// </summary>
public class BadDynamicInteropFunction : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Action<BadExecutionContext> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = context =>
        {
            func(context);

            return Null;
        };
    }

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Action func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = _ =>
        {
            func();

            return Null;
        };
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func(caller);
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction(Func<BadExecutionContext, BadObject> func)
    {
        return new BadDynamicInteropFunction(null,
                                             func,
                                             BadAnyPrototype.Instance
                                            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T">First Argument</typeparam>
public class BadDynamicInteropFunction<T> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T>(Func<BadExecutionContext, T, BadObject> func)
    {
        return new BadDynamicInteropFunction<T>(null,
                                                func,
                                                BadAnyPrototype.Instance,
                                                typeof(T).Name
                                               );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2>(Func<BadExecutionContext, T1, T2, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2>(null,
                                                     func,
                                                     BadAnyPrototype.Instance,
                                                     typeof(T1).Name,
                                                     typeof(T2).Name
                                                    );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3>(
        Func<BadExecutionContext, T1, T2, T3, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3>(null,
                                                         func,
                                                         BadAnyPrototype.Instance,
                                                         typeof(T1).Name,
                                                         typeof(T2).Name,
                                                         typeof(T3).Name
                                                        );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4>(
        Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4>(null,
                                                             func,
                                                             BadAnyPrototype.Instance,
                                                             typeof(T1).Name,
                                                             typeof(T2).Name,
                                                             typeof(T3).Name,
                                                             typeof(T4).Name
                                                            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5>(null,
                                                                 func,
                                                                 BadAnyPrototype.Instance,
                                                                 typeof(T1).Name,
                                                                 typeof(T2).Name,
                                                                 typeof(T3).Name,
                                                                 typeof(T4).Name,
                                                                 typeof(T5).Name
                                                                );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(null,
                                                                     func,
                                                                     BadAnyPrototype.Instance,
                                                                     typeof(T1).Name,
                                                                     typeof(T2).Name,
                                                                     typeof(T3).Name,
                                                                     typeof(T4).Name,
                                                                     typeof(T5).Name,
                                                                     typeof(T6).Name
                                                                    );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(null,
                                                                         func,
                                                                         BadAnyPrototype.Instance,
                                                                         typeof(T1).Name,
                                                                         typeof(T2).Name,
                                                                         typeof(T3).Name,
                                                                         typeof(T4).Name,
                                                                         typeof(T5).Name,
                                                                         typeof(T6).Name,
                                                                         typeof(T7).Name
                                                                        );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(null,
                                                                             func,
                                                                             BadAnyPrototype.Instance,
                                                                             typeof(T1).Name,
                                                                             typeof(T2).Name,
                                                                             typeof(T3).Name,
                                                                             typeof(T4).Name,
                                                                             typeof(T5).Name,
                                                                             typeof(T6).Name,
                                                                             typeof(T7).Name,
                                                                             typeof(T8).Name
                                                                            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name
            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
/// <typeparam name="T10">Tenth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>(),
                                   GetParameter(args, 9)
                                       .Unwrap<T10>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name,
             typeof(T10).Name
            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
/// <typeparam name="T10">Tenth Argument</typeparam>
/// <typeparam name="T11">Eleventh Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject>
                                         func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>(),
                                   GetParameter(args, 9)
                                       .Unwrap<T10>(),
                                   GetParameter(args, 10)
                                       .Unwrap<T11>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name,
             typeof(T10).Name,
             typeof(T11).Name
            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
/// <typeparam name="T10">Tenth Argument</typeparam>
/// <typeparam name="T11">Eleventh Argument</typeparam>
/// <typeparam name="T12">Twelfth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,
                                         BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>(),
                                   GetParameter(args, 9)
                                       .Unwrap<T10>(),
                                   GetParameter(args, 10)
                                       .Unwrap<T11>(),
                                   GetParameter(args, 11)
                                       .Unwrap<T12>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name,
             typeof(T10).Name,
             typeof(T11).Name,
             typeof(T12).Name
            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
/// <typeparam name="T10">Tenth Argument</typeparam>
/// <typeparam name="T11">Eleventh Argument</typeparam>
/// <typeparam name="T12">Twelfth Argument</typeparam>
/// <typeparam name="T13">Thirteenth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject>
        m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
                                         BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>(),
                                   GetParameter(args, 9)
                                       .Unwrap<T10>(),
                                   GetParameter(args, 10)
                                       .Unwrap<T11>(),
                                   GetParameter(args, 11)
                                       .Unwrap<T12>(),
                                   GetParameter(args, 12)
                                       .Unwrap<T13>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name,
             typeof(T10).Name,
             typeof(T11).Name,
             typeof(T12).Name,
             typeof(T13).Name
            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
/// <typeparam name="T10">Tenth Argument</typeparam>
/// <typeparam name="T11">Eleventh Argument</typeparam>
/// <typeparam name="T12">Twelfth Argument</typeparam>
/// <typeparam name="T13">Thirteenth Argument</typeparam>
/// <typeparam name="T14">Fourteenth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject>
        m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
                                         T14, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>(),
                                   GetParameter(args, 9)
                                       .Unwrap<T10>(),
                                   GetParameter(args, 10)
                                       .Unwrap<T11>(),
                                   GetParameter(args, 11)
                                       .Unwrap<T12>(),
                                   GetParameter(args, 12)
                                       .Unwrap<T13>(),
                                   GetParameter(args, 13)
                                       .Unwrap<T14>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator
        BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name,
             typeof(T10).Name,
             typeof(T11).Name,
             typeof(T12).Name,
             typeof(T13).Name,
             typeof(T14).Name
            );
    }
}

/// <summary>
///     Generic Interop Function.
/// </summary>
/// <typeparam name="T1">First Argument</typeparam>
/// <typeparam name="T2">Second Argument</typeparam>
/// <typeparam name="T3">Third Argument</typeparam>
/// <typeparam name="T4">Forth Argument</typeparam>
/// <typeparam name="T5">Fifth Argument</typeparam>
/// <typeparam name="T6">Sixth Argument</typeparam>
/// <typeparam name="T7">Seventh Argument</typeparam>
/// <typeparam name="T8">Eighth Argument</typeparam>
/// <typeparam name="T9">Ninth Argument</typeparam>
/// <typeparam name="T10">Tenth Argument</typeparam>
/// <typeparam name="T11">Eleventh Argument</typeparam>
/// <typeparam name="T12">Twelfth Argument</typeparam>
/// <typeparam name="T13">Thirteenth Argument</typeparam>
/// <typeparam name="T14">Fourteenth Argument</typeparam>
/// <typeparam name="T15">Fifteenth Argument</typeparam>
public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : BadFunction
{
    /// <summary>
    ///     The Function Lambda
    /// </summary>
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15,
        BadObject> m_Func;

    /// <summary>
    ///     Creates a new BadDynamicInteropFunction
    /// </summary>
    /// <param name="name">The Name of the Function</param>
    /// <param name="func">The Function Lambda</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The Parameters of the Function</param>
    public BadDynamicInteropFunction(BadWordToken? name,
                                     Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,
                                         T14, T15, BadObject> func,
                                     BadClassPrototype returnType,
                                     params BadFunctionParameter[] parameters) : base(name,
                                                                                      false,
                                                                                      false,
                                                                                      returnType,
                                                                                      false,
                                                                                      parameters
                                                                                     )
    {
        m_Func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args, caller);

        yield return m_Func.Invoke(caller,
                                   GetParameter(args, 0)
                                       .Unwrap<T1>(),
                                   GetParameter(args, 1)
                                       .Unwrap<T2>(),
                                   GetParameter(args, 2)
                                       .Unwrap<T3>(),
                                   GetParameter(args, 3)
                                       .Unwrap<T4>(),
                                   GetParameter(args, 4)
                                       .Unwrap<T5>(),
                                   GetParameter(args, 5)
                                       .Unwrap<T6>(),
                                   GetParameter(args, 6)
                                       .Unwrap<T7>(),
                                   GetParameter(args, 7)
                                       .Unwrap<T8>(),
                                   GetParameter(args, 8)
                                       .Unwrap<T9>(),
                                   GetParameter(args, 9)
                                       .Unwrap<T10>(),
                                   GetParameter(args, 10)
                                       .Unwrap<T11>(),
                                   GetParameter(args, 11)
                                       .Unwrap<T12>(),
                                   GetParameter(args, 12)
                                       .Unwrap<T13>(),
                                   GetParameter(args, 13)
                                       .Unwrap<T14>(),
                                   GetParameter(args, 14)
                                       .Unwrap<T15>()
                                  );
    }

    /// <summary>
    ///     Converts a Function Lambda to a BadDynamicInteropFunction
    /// </summary>
    /// <param name="func">The Function Lambda</param>
    /// <returns>A new BadDynamicInteropFunction</returns>
    public static implicit operator
        BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(null,
             func,
             BadAnyPrototype.Instance,
             typeof(T1).Name,
             typeof(T2).Name,
             typeof(T3).Name,
             typeof(T4).Name,
             typeof(T5).Name,
             typeof(T6).Name,
             typeof(T7).Name,
             typeof(T8).Name,
             typeof(T9).Name,
             typeof(T10).Name,
             typeof(T11).Name,
             typeof(T12).Name,
             typeof(T13).Name,
             typeof(T14).Name,
             typeof(T15).Name
            );
    }
}