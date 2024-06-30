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
    /// <summary>
    ///     The Prototype for the Function Object
    /// </summary>
    public static readonly BadClassPrototype Prototype = BadNativeClassBuilder.GetNative("Function");

    /// <summary>
    ///     The Result Cache
    /// </summary>
    private readonly Dictionary<int, BadObject> m_Cache = new Dictionary<int, BadObject>();

    /// <summary>
    ///     Creates a new Function
    /// </summary>
    /// <param name="name">(optional) Function Name</param>
    /// <param name="isConstant">Indicates if the function has no side effects and the result can be cached</param>
    /// <param name="returnType">The Return Type of the Function</param>
    /// <param name="parameters">The function parameters</param>
    /// <param name="isStatic">Is the Function static</param>
    protected BadFunction(
        BadWordToken? name,
        bool isConstant,
        bool isStatic,
        BadClassPrototype returnType, 
        bool isSingleLine, 
        params BadFunctionParameter[] parameters)
    {
        Name = name;
        IsConstant = isConstant;
        IsStatic = isStatic;
        Parameters = parameters;
        ReturnType = returnType;
        IsSingleLine = isSingleLine;
    }

    /// <summary>
    ///     The Return Type of the Function
    /// </summary>
    public BadClassPrototype ReturnType { get; }

    /// <summary>
    ///     Indicates if the Function is static
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    ///     The Metadata of the Function
    /// </summary>
    public virtual BadMetaData MetaData => BadMetaData.Empty;

    /// <summary>
    ///     Indicates if the function has no side effects and the result can be cached
    /// </summary>
    public bool IsConstant { get; }
    
    /// <summary>
    /// Indicates if the function is a single line function(without block)
    /// Is used to determine the behaviour of the return statement
    /// The Runtime always wraps single line function bodies into a return statement
    /// e.g. function f() => true; is transformed into function f() { return true; }
    /// If the function declares the return type as void, the return statement does evaluate the inner expression,
    /// but does not set the result in the scope. It sets a special BadObject (BadVoidPrototype.Object) as result)
    /// If a void prototype is defined as return type, the function is NOT a single line function and the return statement has an inner expression, an exception is raised.
    /// </summary>
    public bool IsSingleLine { get; }

    /// <summary>
    ///     (optional) Name of the Function
    /// </summary>
    public BadWordToken? Name { get; }

    /// <summary>
    ///     The Function Parameters
    /// </summary>
    public BadFunctionParameter[] Parameters { get; }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
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
    ///     Returns the Function Parameter at the given index
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <param name="i">Index</param>
    /// <returns>The Function Parameter at the given index</returns>
    protected static BadObject GetParameter(BadObject[] args, int i)
    {
        return args.Length > i ? args[i].Dereference() : Null;
    }

    /// <summary>
    ///     Checks Parameters for the given function call
    /// </summary>
    /// <param name="args">Arguments provided for the invocation</param>
    /// <param name="caller">The Caller Context</param>
    /// <param name="position">The Source position</param>
    /// <exception cref="BadRuntimeException">Gets raised if the Parameters are invalid for the function</exception>
    protected void CheckParameters(BadObject[] args, BadExecutionContext caller, BadSourcePosition? position = null)
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
                    throw BadRuntimeException.Create(caller.Scope, $"Wrong number of parameters for '{this}'. Expected Argument for '{parameter}'", position);
                }
            }
            else
            {
                if (parameter.IsNullChecked && args[i] == Null)
                {
                    throw BadRuntimeException.Create(caller.Scope, $"Null value not allowed for '{this}' parameter '{parameter}'", position);
                }
            }
        }
    }

    /// <summary>
    ///     Applies the function arguments to the context of the function
    /// </summary>
    /// <param name="funcStr">The Function String</param>
    /// <param name="parameters">The Function Parameters</param>
    /// <param name="context">The Function Context</param>
    /// <param name="args">The Function Arguments</param>
    /// <param name="position">The Source Position used for raising exceptions</param>
    /// <exception cref="BadRuntimeException">Gets raised if the arguments can not be set in the context.</exception>
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
                    context.Scope.DefineVariable(parameter.Name, Null, null, new BadPropertyInfo(parameter.Type ?? BadAnyPrototype.Instance));
                }
                else
                {
                    throw BadRuntimeException.Create(context.Scope, $"Wrong number of parameters for '{funcStr}'. Expected Argument for '{parameter}'", position);
                }
            }
            else
            {
                if (parameter.IsNullChecked && args[i] == Null)
                {
                    throw BadRuntimeException.Create(context.Scope, $"Null value not allowed for '{funcStr}' parameter '{parameter}'", position);
                }

                context.Scope.DefineVariable(parameter.Name, args[i], null, new BadPropertyInfo(parameter.Type ?? BadAnyPrototype.Instance));
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
    private static int? GetHash(IEnumerable<BadObject> args)
    {
        int hash = 0;

        //Generate Hash from arguments
        foreach (BadObject o in args)
        {
            if (o is not IBadNative native)
            {
                return null;
            }

            hash = hash == 0 ? native.Value.GetHashCode() : BadHashCode.Combine(hash, native.Value);
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

            if (hash != null && m_Cache.TryGetValue(hash.Value, out BadObject? v))
            {
                BadLogger.Warn($"Found Cached value with Hash {hash}", "Runtime");

                yield return v;

                yield break;
            }
        }

        BadObject? ret = null;

        foreach (BadObject o in InvokeBlock(args, caller))
        {
            ret = o;

            yield return o;
        }
        

        if (ret != null && !ReturnType.IsAssignableFrom(ret))
        {
            throw new BadRuntimeException(
                $"Invalid return type for function '{GetHeader()}'. Expected '{ReturnType.Name}' got '{ret.GetPrototype().Name}'"
            );
        }

        if (!IsConstant ||
            ret == null ||
            !BadNativeOptimizationSettings.Instance.UseConstantFunctionCaching)
        {
            yield break;
        }

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
        return GetHeader(Name?.ToString() ?? "<anonymous>", ReturnType, Parameters);
    }

    /// <summary>
    ///     Returns the Header of the function
    /// </summary>
    /// <param name="name">The Function Name</param>
    /// <param name="returnType">The Return Type</param>
    /// <param name="parameters">The Function Parameters</param>
    /// <returns></returns>
    public static string GetHeader(string name, BadClassPrototype returnType, IEnumerable<BadFunctionParameter> parameters)
    {
        return $"{BadStaticKeys.FUNCTION_KEY} {returnType.Name} {name}({string.Join(", ", parameters.Cast<object>())})";
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return GetHeader();
    }
}