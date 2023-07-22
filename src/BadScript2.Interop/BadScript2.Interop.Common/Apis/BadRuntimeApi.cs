using System.Reflection;
using System.Runtime.InteropServices;

using BadScript2.Interop.Common.Task;
using BadScript2.Optimizations;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Settings;

namespace BadScript2.Interop.Common.Apis;

/// <summary>
///     Implements the "Runtime" API
/// </summary>
public class BadRuntimeApi : BadInteropApi
{
	/// <summary>
	///     The Exported Objects
	/// </summary>
	private readonly Dictionary<string, BadObject> m_Exports = new Dictionary<string, BadObject>();

	/// <summary>
	///     Constructs a new Runtime API Instance
	/// </summary>
	public BadRuntimeApi() : base("Runtime") { }

	/// <summary>
	///     The Startup Arguments that were passed to the Runtime
	/// </summary>
	public static IEnumerable<string>? StartupArguments { get; set; }

	/// <summary>
	///     Creates the "Native" Table
	/// </summary>
	/// <returns>Bad Table</returns>
	private BadTable MakeNative()
    {
        BadTable table = new BadTable();

        table.SetFunction<string>("ParseNumber", s => decimal.Parse(s));
        table.SetFunction<BadObject>("IsNative", Native_IsNative);
        table.SetFunction<BadObject>("IsFunction", Native_IsFunction);
        table.SetFunction<BadObject>("IsTable", Native_IsTable);
        table.SetFunction<BadObject>("IsString", Native_IsString);
        table.SetFunction<BadObject>("IsNumber", Native_IsNumber);
        table.SetFunction<BadObject>("IsBoolean", Native_IsBoolean);
        table.SetFunction<BadObject>("IsArray", Native_IsArray);
        table.SetFunction<BadObject>("IsEnumerable", Native_IsEnumerable);
        table.SetFunction<BadObject>("IsEnumerator", Native_IsEnumerator);
        table.SetFunction<BadObject>("IsPrototype", Native_IsPrototype);
        table.SetFunction<BadObject>("IsPrototypeInstance", Native_IsPrototypeInstance);

        return table;
    }


	/// <summary>
	///     Returns true if the given object is an instance of a class prototype
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsPrototypeInstance(BadObject arg)
    {
        return arg is BadClass;
    }

	/// <summary>
	///     Returns true if the given object is a class prototype
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsPrototype(BadObject arg)
    {
        return arg is BadClassPrototype;
    }

	/// <summary>
	///     Returns true if the given object is a native object
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsNative(BadObject arg)
    {
        return arg is IBadNative;
    }

	/// <summary>
	///     Returns true if the given object is a function
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsFunction(BadObject arg)
    {
        return arg is BadFunction;
    }

	/// <summary>
	///     Returns true if the given object is a table
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsTable(BadObject arg)
    {
        return arg is BadTable;
    }

	/// <summary>
	///     Returns true if the given object is a string
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsString(BadObject arg)
    {
        return arg is IBadString;
    }

	/// <summary>
	///     Returns true if the given object is a number
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsNumber(BadObject arg)
    {
        return arg is IBadNumber;
    }

	/// <summary>
	///     Returns true if the given object is a boolean
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsBoolean(BadObject arg)
    {
        return arg is IBadBoolean;
    }

	/// <summary>
	///     Returns true if the given object is an array
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsArray(BadObject arg)
    {
        return arg is BadArray;
    }

	/// <summary>
	///     Returns true if the given object is enumerable
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsEnumerable(BadObject arg)
    {
        return arg.HasProperty("GetEnumerator");
    }

	/// <summary>
	///     Returns true if the given object is an enumerator
	/// </summary>
	/// <param name="arg">Object to test</param>
	/// <returns>Boolean</returns>
	private BadObject Native_IsEnumerator(BadObject arg)
    {
        return arg.HasProperty("GetCurrent") && arg.HasProperty("MoveNext");
    }


    protected override void LoadApi(BadTable target)
    {
        target.SetProperty(
            "Evaluate",
            new BadInteropFunction(
                "Evaluate",
                (ctx, args) => Evaluate(
                    ctx,
                    args[0],
                    args.Length < 2 ? BadObject.Null : args[1],
                    args.Length < 3 ? BadObject.True : args[2],
                    args.Length < 4 ? BadObject.Null : args[3]
                ),
                false,
                "src",
                new BadFunctionParameter("file", true, false, false),
                new BadFunctionParameter("optimize", true, false, false),
                new BadFunctionParameter("scope", true, false, false)
            )
        );
        target.SetProperty(
            "EvaluateAsync",
            new BadInteropFunction(
                "EvaluateAsync",
                (ctx, args) => EvaluateAsync(
                    ctx,
                    args[0],
                    args.Length < 2 ? BadObject.Null : args[1],
                    args.Length < 3 ? BadObject.True : args[2],
                    args.Length < 4 ? BadObject.Null : args[3],
                    args.Length < 5 ? BadObject.False : args[4]
                ),
                false,
                "src",
                new BadFunctionParameter("file", true, false, false),
                new BadFunctionParameter("optimize", true, false, false),
                new BadFunctionParameter("scope", true, false, false),
                new BadFunctionParameter("setLastAsReturn", true, false, false)
            )
        );
        target.SetFunction(
            "CreateDefaultScope",
            ctx => ctx.Scope.GetRootScope().CreateChild("<customscope>", ctx.Scope, null)
        );
        target.SetFunction("GetStackTrace", ctx => ctx.Scope.GetStackTrace());
        target.SetProperty("Native", MakeNative());
        target.SetFunction<string, BadObject>("Export", Export);
        target.SetFunction<string>("Import", Import);
        target.SetFunction<string>("HasPackage", HasPackage);
        target.SetFunction("GetArguments", GetArguments);
        target.SetFunction<BadObject>("GetExtensionNames", GetExtensionNames);
        target.SetFunction("GetGlobalExtensionNames", GetGlobalExtensionNames);
        target.SetFunction("GetTimeNow", GetTimeNow);
        target.SetFunction(
            "GetNativeTypes",
            _ => new BadArray(BadNativeClassBuilder.NativeTypes.Cast<BadObject>().ToList())
        );
        target.SetFunction(
            "GetRuntimeAssemblyPath",
            () =>
            {
                string path;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    path = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "exe");
                }
                else
                {
                    path = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "");
                }

                return path;
            }
        );
    }


    /// <summary>
    ///     Returns the Current Time
    /// </summary>
    /// <returns>Bad Table with the Current Time</returns>
    private static BadObject GetTimeNow()
    {
        BadTable table = new BadTable();
        DateTime time = DateTime.Now;
        table.SetProperty("Year", time.Year);
        table.SetProperty("Month", time.Month);
        table.SetProperty("Day", time.Day);
        table.SetProperty("Hour", time.Hour);
        table.SetProperty("Minute", time.Minute);
        table.SetProperty("Second", time.Second);
        table.SetProperty("Millisecond", time.Millisecond);
        table.SetProperty("UnixTimeMilliseconds", ((DateTimeOffset)time).ToUnixTimeMilliseconds());
        table.SetProperty("UnixTimeSeconds", ((DateTimeOffset)time).ToUnixTimeSeconds());

        return table;
    }

    /// <summary>
    ///     Returns all Extension Names of the given object
    /// </summary>
    /// <param name="o">Object</param>
    /// <returns>Array of Extension Names</returns>
    private BadObject GetExtensionNames(BadObject o)
    {
        return new BadArray(BadInteropExtension.GetExtensionNames(o).ToList());
    }

    /// <summary>
    ///     Lists all global extension names
    /// </summary>
    /// <returns>Array of Extension Names</returns>
    private BadObject GetGlobalExtensionNames()
    {
        return new BadArray(BadInteropExtension.GetExtensionNames().ToList());
    }

    /// <summary>
    ///     Returns the arguments passed to the script
    /// </summary>
    /// <returns>Array of Arguments</returns>
    private BadObject GetArguments()
    {
        return StartupArguments == null ? new BadArray() : new BadArray(StartupArguments.Select(x => (BadObject)x).ToList());
    }

    /// <summary>
    ///     Exports a package
    /// </summary>
    /// <param name="name">Package Name</param>
    /// <param name="obj">Package</param>
    private void Export(string name, BadObject obj)
    {
        m_Exports.Add(name, obj);
    }

    /// <summary>
    ///     Returns true if the given package is exported
    /// </summary>
    /// <param name="name">Package Name</param>
    /// <returns>True if package exists</returns>
    private BadObject HasPackage(string name)
    {
        return m_Exports.ContainsKey(name);
    }

    /// <summary>
    ///     Imports a package
    /// </summary>
    /// <param name="name">Package Name</param>
    /// <returns>Package</returns>
    private BadObject Import(string name)
    {
        return m_Exports[name];
    }

    /// <summary>
    ///     DONT USE THIS! USE THE ASYNC VERSION!
    ///     Evaluates a string
    /// </summary>
    /// <param name="caller">Caller Context</param>
    /// <param name="str">Source String</param>
    /// <param name="fileObj">The File Name</param>
    /// <param name="optimizeExpr">Boolean that determines if the source will be optimized before execution</param>
    /// <param name="scope">The Scope that the source will be executed in</param>
    /// <returns>Result of the Source</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Arguments are invalid</exception>
    private BadObject Evaluate(
        BadExecutionContext caller,
        BadObject str,
        BadObject fileObj,
        BadObject optimizeExpr,
        BadObject scope)
    {
        if (str is not IBadString src)
        {
            throw new BadRuntimeException($"Evaluate: Argument 'src' is not a string {str}");
        }

        if (optimizeExpr is not IBadBoolean optimizeE)
        {
            throw new BadRuntimeException($"Evaluate: Argument 'optimize' is not a boolean {optimizeExpr}");
        }

        string file = "<eval>";

        if (fileObj is IBadString fileStr)
        {
            file = fileStr.Value;
        }
        else if (fileObj != BadObject.Null)
        {
            throw new BadRuntimeException("Evaluate: Argument 'fileObj' is not a string");
        }


        bool optimize = BadNativeOptimizationSettings.Instance.UseConstantExpressionOptimization && optimizeE.Value;

        BadExecutionContext ctx =
            scope == BadObject.Null || scope is not BadScope sc
                ? new BadExecutionContext(
                    caller.Scope.GetRootScope()
                        .CreateChild("<EvaluateAsync>", caller.Scope, null)
                )
                : new BadExecutionContext(sc);

        IEnumerable<BadExpression> exprs = BadSourceParser.Create(file, src.Value).Parse();

        if (optimize)
        {
            exprs = BadExpressionOptimizer.Optimize(exprs);
        }

        return ctx.Run(exprs) ?? BadObject.Null;
    }

    /// <summary>
    ///     Evaluates a string
    /// </summary>
    /// <param name="caller">Caller Context</param>
    /// <param name="str">Source String</param>
    /// <param name="fileObj">The File Name</param>
    /// <param name="optimizeExpr">Boolean that determines if the source will be optimized before execution</param>
    /// <param name="scope">The Scope that the source will be executed in</param>
    /// <param name="setLastAsReturn">If true, the last result of the Source Enumeration will be set as the Task Result</param>
    /// <returns>Awaitable Result</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Arguments are invalid</exception>
    private BadObject EvaluateAsync(
        BadExecutionContext caller,
        BadObject str,
        BadObject fileObj,
        BadObject optimizeExpr,
        BadObject scope,
        BadObject setLastAsReturn)
    {
        if (str is not IBadString src)
        {
            throw new BadRuntimeException($"Evaluate: Argument 'src' is not a string {str}");
        }

        if (optimizeExpr is not IBadBoolean optimizeE)
        {
            throw new BadRuntimeException($"Evaluate: Argument 'optimize' is not a boolean {optimizeExpr}");
        }

        string file = "<eval>";

        if (fileObj is IBadString fileStr)
        {
            file = fileStr.Value;
        }
        else if (fileObj != BadObject.Null)
        {
            throw new BadRuntimeException("Evaluate: Argument 'fileObj' is not a string");
        }

        if (setLastAsReturn is not IBadBoolean setLastAsReturnB)
        {
            throw new BadRuntimeException("Evaluate: Argument 'setLastAsReturn' is not a boolean");
        }


        bool optimize = BadNativeOptimizationSettings.Instance.UseConstantExpressionOptimization && optimizeE.Value;

        BadExecutionContext ctx =
            scope == BadObject.Null || scope is not BadScope sc
                ? new BadExecutionContext(
                    caller.Scope.GetRootScope()
                        .CreateChild("<EvaluateAsync>", caller.Scope, null)
                )
                : new BadExecutionContext(sc);

        IEnumerable<BadExpression> exprs = BadSourceParser.Create(file, src.Value).Parse();

        if (optimize)
        {
            exprs = BadExpressionOptimizer.Optimize(exprs);
        }


        return new BadTask(
            new BadInteropRunnable(SafeExecute(ctx.Execute(exprs), ctx).GetEnumerator(), setLastAsReturnB.Value),
            "EvaluateAsync"
        );
    }

    private IEnumerable<BadObject> SafeExecute(IEnumerable<BadObject> script, BadExecutionContext ctx)
    {
        foreach (BadObject o in script)
        {
            if (ctx.Scope.IsError)
            {
                throw new BadRuntimeErrorException(ctx.Scope.Error);
            }

            yield return o;
        }
    }
}