using BadScript2.Common;
using BadScript2.Common.Logging;
using BadScript2.Parser;
using BadScript2.Reader.Token;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;
using BadScript2.Utility;

namespace BadScript2.Runtime.Objects.Functions;

/// <summary>
///     Implements a function that can be called from the script
/// </summary>
public abstract class BadFunction : BadObject
{
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("Function");

    /// <summary>
    ///     The Result Cache
    /// </summary>
    private readonly Dictionary<int, BadObject> m_Cache = new Dictionary<int, BadObject>();

    /// <summary>
    ///     Creates a new Function
    /// </summary>
    /// <param name="name">(optional) Function Name</param>
    /// <param name="isConstant">Indicates if the function has no side effects and the result can be cached</param>
    /// <param name="parameters">The function parameters</param>
    protected BadFunction(BadWordToken? name, bool isConstant, bool isStatic, params BadFunctionParameter[] parameters)
    {
        Name = name;
        IsConstant = isConstant;
        IsStatic = isStatic;
        Parameters = parameters;
    }

    public bool IsStatic { get; }

    public virtual BadMetaData MetaData => BadMetaData.Empty;

    /// <summary>
    ///     Indicates if the function has no side effects and the result can be cached
    /// </summary>
    public bool IsConstant { get; }

    /// <summary>
    ///     (optional) Name of the Function
    /// </summary>
    public BadWordToken? Name { get; }

    /// <summary>
    ///     The Function Parameters
    /// </summary>
    public BadFunctionParameter[] Parameters { get; }

    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <summary>
    ///     Returns an instance that is bound to the given scope
    /// </summary>
    /// <param name="scope">Scope to bind to</param>
    /// <returns>Function Instance</returns>
    public virtual BadFunction BindParentScope(BadScope scope)
    {
        return this;
    }

    /// <summary>
    ///     Checks Parameters for the given function call
    /// </summary>
    /// <param name="args">Arguments provided for the invocation</param>
    /// <exception cref="BadRuntimeException">Gets raised if the Parameters are invalid for the function</exception>
    protected void CheckParameters(BadObject[] args)
    {
        for (int i = 0; i < Parameters.Length; i++)
        {
            BadFunctionParameter parameter = Parameters[i];

            if (parameter.IsRestArgs)
            {
                //Do Nothing
            }
            else if (args.Length <= i)
            {
                if (!parameter.IsOptional)
                {
                    throw new BadRuntimeException(
                        $"Wrong number of parameters for '{this}'. Expected Argument for '{parameter}'"
                    );
                }
            }
            else
            {
                if (parameter.IsNullChecked && args[i] == Null)
                {
                    throw new BadRuntimeException($"Null value not allowed for '{this}' parameter '{parameter}'");
                }
            }
        }
    }

    public static void ApplyParameters(
        string funcStr,
        BadFunctionParameter[] parameters,
        BadExecutionContext context,
        BadObject[] args,
        BadSourcePosition? position = null)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            BadFunctionParameter parameter = parameters[i];

            if (parameter.IsRestArgs)
            {
                context.Scope.DefineVariable(
                    parameter.Name,
                    new BadArray(args.Skip(i).ToList()),
                    null,
                    new BadPropertyInfo(BadNativeClassBuilder.GetNative("Array"))
                );
            }
            else if (args.Length <= i)
            {
                if (parameter.IsOptional)
                {
                    context.Scope.DefineVariable(parameter.Name, Null, null, new BadPropertyInfo(parameter.Type));
                }
                else
                {
                    if (position != null)
                    {
                        throw new BadRuntimeException(
                            $"Wrong number of parameters for '{funcStr}'. Expected Argument for '{parameter}'",
                            position
                        );
                    }

                    throw new BadRuntimeException(
                        $"Wrong number of parameters for '{funcStr}'. Expected Argument for '{parameter}'"
                    );
                }
            }
            else
            {
                if (parameter.IsNullChecked && args[i] == Null)
                {
                    if (position != null)
                    {
                        throw new BadRuntimeException(
                            $"Null value not allowed for '{funcStr}' parameter '{parameter}'",
                            position
                        );
                    }

                    throw new BadRuntimeException($"Null value not allowed for '{funcStr}' parameter '{parameter}'");
                }

                context.Scope.DefineVariable(parameter.Name, args[i], null, new BadPropertyInfo(parameter.Type));
            }
        }
    }

    /// <summary>
    ///     Applies the function arguments to the context of the function
    /// </summary>
    /// <param name="context">Function Context</param>
    /// <param name="args">Arguments</param>
    /// <param name="position">Source Position used for raising exceptions</param>
    /// <exception cref="BadRuntimeException">Gets raised if the arguments can not be set in the context.</exception>
    public void ApplyParameters(BadExecutionContext context, BadObject[] args, BadSourcePosition? position = null)
    {
        ApplyParameters(ToString(), Parameters, context, args, position);
    }

    /// <summary>
    ///     Returns the Hash of the function arguments
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>The Hash for the arguments</returns>
    private int? GetHash(BadObject[] args)
    {
        int hash = 0;

        //Generate Hash from arguments
        foreach (BadObject o in args)
        {
            if (o is not IBadNative native)
            {
                return null;
            }

            if (hash == 0)
            {
                hash = native.Value.GetHashCode();
            }
            else
            {
                hash = BadHashCode.Combine(hash, native.Value);
            }
        }

        return hash;
    }

    /// <summary>
    ///     Invokes the function with the specified arguments
    /// </summary>
    /// <param name="args">Function Arguments</param>
    /// <param name="caller">The Calling Context</param>
    /// <returns>Enumeration of BadObjects</returns>
    public IEnumerable<BadObject> Invoke(BadObject[] args, BadExecutionContext caller)
    {
        if (IsConstant && BadNativeOptimizationSettings.Instance.UseConstantFunctionCaching)
        {
            int? hash = GetHash(args);

            if (hash != null && m_Cache.ContainsKey(hash.Value))
            {
                BadLogger.Warn($"Found Cached value with Hash {hash}", "Runtime");

                yield return m_Cache[hash.Value];

                yield break;
            }
        }

        BadObject? ret = null;

        foreach (BadObject o in InvokeBlock(args, caller))
        {
            ret = o;

            yield return o;
        }

        if (!caller.Scope.IsError &&
            IsConstant &&
            ret != null &&
            BadNativeOptimizationSettings.Instance.UseConstantFunctionCaching)
        {
            int? hash = GetHash(args);

            if (hash != null)
            {
                //BadLogger.Warn($"Caching Result {ret.ToSafeString()} for function '{GetHeader()}'", "Runtime");
                m_Cache[hash.Value] = ret;
            }
        }
    }

    /// <summary>
    ///     Invokes the function with the specified arguments
    /// </summary>
    /// <param name="args">Function Arguments</param>
    /// <param name="caller">The Calling Context</param>
    /// <returns>Enumeration of BadObjects</returns>
    protected abstract IEnumerable<BadObject> InvokeBlock(BadObject[] args, BadExecutionContext caller);


    /// <summary>
    ///     Returns the Header of the function
    /// </summary>
    /// <returns>String Header</returns>
    public string GetHeader()
    {
        return GetHeader(Name?.ToString() ?? "<anonymous>", Parameters);
    }

    public static string GetHeader(string name, BadFunctionParameter[] parameters)
    {
        return $"{BadStaticKeys.FunctionKey} {name}({string.Join(", ", parameters.Cast<object>())})";
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return GetHeader();
    }
}