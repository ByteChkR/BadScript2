using BadScript2.Reader.Token;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Runtime.Interop.Functions;

public class BadDynamicInteropFunction : BadFunction
{
    private readonly Func<BadExecutionContext, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Action<BadExecutionContext> func,
        params BadFunctionParameter[] parameters) : base(
        name,
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

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Action func,
        params BadFunctionParameter[] parameters) : base(
        name,
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

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func(caller);
    }

    public static implicit operator BadDynamicInteropFunction(Func<BadExecutionContext, BadObject> func)
    {
        return new BadDynamicInteropFunction(
            null,
            func
        );
    }


    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T> : BadFunction
{
    private readonly Func<BadExecutionContext, T, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(caller, args[0].Dereference().Unwrap<T>());
    }

    public static implicit operator BadDynamicInteropFunction<T>(Func<BadExecutionContext, T, BadObject> func)
    {
        return new BadDynamicInteropFunction<T>(
            null,
            func,
            typeof(T).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2>(Func<BadExecutionContext, T1, T2, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2>(
            null,
            func,
            typeof(T1).Name,
            typeof(T2).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3>(
        Func<BadExecutionContext, T1, T2, T3, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3>(
            null,
            func,
            typeof(T1).Name,
            typeof(T2).Name,
            typeof(T3).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4>(
        Func<BadExecutionContext, T1, T2, T3, T4, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4>(
            null,
            func,
            typeof(T1).Name,
            typeof(T2).Name,
            typeof(T3).Name,
            typeof(T4).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5>(
            null,
            func,
            typeof(T1).Name,
            typeof(T2).Name,
            typeof(T3).Name,
            typeof(T4).Name,
            typeof(T5).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6>(
            null,
            func,
            typeof(T1).Name,
            typeof(T2).Name,
            typeof(T3).Name,
            typeof(T4).Name,
            typeof(T5).Name,
            typeof(T6).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7>(
            null,
            func,
            typeof(T1).Name,
            typeof(T2).Name,
            typeof(T3).Name,
            typeof(T4).Name,
            typeof(T5).Name,
            typeof(T6).Name,
            typeof(T7).Name
        );
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>(),
            args[9].Dereference().Unwrap<T10>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>(),
            args[9].Dereference().Unwrap<T10>(),
            args[10].Dereference().Unwrap<T11>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>(),
            args[9].Dereference().Unwrap<T10>(),
            args[10].Dereference().Unwrap<T11>(),
            args[11].Dereference().Unwrap<T12>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject>
        m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>(),
            args[9].Dereference().Unwrap<T10>(),
            args[10].Dereference().Unwrap<T11>(),
            args[11].Dereference().Unwrap<T12>(),
            args[12].Dereference().Unwrap<T13>()
        );
    }

    public static implicit operator BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject>
        m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>(),
            args[9].Dereference().Unwrap<T10>(),
            args[10].Dereference().Unwrap<T11>(),
            args[11].Dereference().Unwrap<T12>(),
            args[12].Dereference().Unwrap<T13>(),
            args[13].Dereference().Unwrap<T14>()
        );
    }

    public static implicit operator
        BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}

public class BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : BadFunction
{
    private readonly Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15,
        BadObject> m_Func;

    public BadDynamicInteropFunction(
        BadWordToken? name,
        Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func,
        params BadFunctionParameter[] parameters) : base(
        name,
        false,
        parameters
    )
    {
        m_Func = func;
    }

    protected override IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller)
    {
        CheckParameters(args);

        yield return m_Func.Invoke(
            caller,
            args[0].Dereference().Unwrap<T1>(),
            args[1].Dereference().Unwrap<T2>(),
            args[2].Dereference().Unwrap<T3>(),
            args[3].Dereference().Unwrap<T4>(),
            args[4].Dereference().Unwrap<T5>(),
            args[5].Dereference().Unwrap<T6>(),
            args[6].Dereference().Unwrap<T7>(),
            args[7].Dereference().Unwrap<T8>(),
            args[8].Dereference().Unwrap<T9>(),
            args[9].Dereference().Unwrap<T10>(),
            args[10].Dereference().Unwrap<T11>(),
            args[11].Dereference().Unwrap<T12>(),
            args[12].Dereference().Unwrap<T13>(),
            args[13].Dereference().Unwrap<T14>(),
            args[14].Dereference().Unwrap<T15>()
        );
    }

    public static implicit operator
        BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Func<BadExecutionContext, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, BadObject> func)
    {
        return new BadDynamicInteropFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            null,
            func,
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

    public override string ToSafeString(List<BadObject> done)
    {
        return "<interop> " + base.ToSafeString(done);
    }
}