using System.Reflection;
using System.Runtime.InteropServices;

using BadScript2.Interop.Common.Task;
using BadScript2.IO;
using BadScript2.Optimizations.Folding;
using BadScript2.Optimizations.Substitution;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Validation;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Module;
using BadScript2.Runtime.Module.Handlers;
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

        table.SetFunction<string>("ParseNumber", s => decimal.Parse(s), BadNativeClassBuilder.GetNative("num"));
        table.SetFunction<BadNullable<BadObject>>("IsNative", Native_IsNative, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsFunction", Native_IsFunction, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsTable", Native_IsTable, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsString", Native_IsString, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsNumber", Native_IsNumber, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsBoolean", Native_IsBoolean, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsArray", Native_IsArray, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsEnumerable", Native_IsEnumerable, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsEnumerator", Native_IsEnumerator, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>("IsPrototype", Native_IsPrototype, BadNativeClassBuilder.GetNative("bool"));
        table.SetFunction<BadNullable<BadObject>>(
            "IsPrototypeInstance",
            Native_IsPrototypeInstance,
            BadNativeClassBuilder.GetNative("bool")
        );

        return table;
    }


    /// <summary>
    ///     Returns true if the given object is an instance of a class prototype
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsPrototypeInstance(BadNullable<BadObject> arg)
    {
        return arg.Value is BadClass;
    }

    /// <summary>
    ///     Returns true if the given object is a class prototype
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsPrototype(BadNullable<BadObject> arg)
    {
        return arg.Value is BadClassPrototype;
    }

    /// <summary>
    ///     Returns true if the given object is a native object
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsNative(BadNullable<BadObject> arg)
    {
        return arg.Value is IBadNative;
    }

    /// <summary>
    ///     Returns true if the given object is a function
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsFunction(BadNullable<BadObject> arg)
    {
        return arg.Value is BadFunction;
    }

    /// <summary>
    ///     Returns true if the given object is a table
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsTable(BadNullable<BadObject> arg)
    {
        return arg.Value is BadTable;
    }

    /// <summary>
    ///     Returns true if the given object is a string
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsString(BadNullable<BadObject> arg)
    {
        return arg.Value is IBadString;
    }

    /// <summary>
    ///     Returns true if the given object is a number
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsNumber(BadNullable<BadObject> arg)
    {
        return arg.Value is IBadNumber;
    }

    /// <summary>
    ///     Returns true if the given object is a boolean
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsBoolean(BadNullable<BadObject> arg)
    {
        return arg.Value is IBadBoolean;
    }

    /// <summary>
    ///     Returns true if the given object is an array
    /// </summary>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsArray(BadNullable<BadObject> arg)
    {
        return arg.Value is BadArray;
    }

    /// <summary>
    ///     Returns true if the given object is enumerable
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsEnumerable(BadExecutionContext ctx, BadNullable<BadObject> arg)
    {
        return arg.Value?.HasProperty("GetEnumerator", ctx.Scope) ?? false;
    }

    /// <summary>
    ///     Returns true if the given object is an enumerator
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="arg">Object to test</param>
    /// <returns>Boolean</returns>
    private static BadObject Native_IsEnumerator(BadExecutionContext ctx, BadNullable<BadObject> arg)
    {
        return (arg.Value?.HasProperty("GetCurrent", ctx.Scope) ?? false) &&
               (arg.Value?.HasProperty("MoveNext", ctx.Scope) ?? false);
    }


    /// <inheritdoc />
    protected override void LoadApi(BadTable target)
    {
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
                BadTask.Prototype,
                new BadFunctionParameter("src", false, true, false, null, BadNativeClassBuilder.GetNative("string")),
                new BadFunctionParameter("file", true, false, false, null, BadNativeClassBuilder.GetNative("string")),
                new BadFunctionParameter("optimize", true, false, false, null, BadNativeClassBuilder.GetNative("bool")),
                new BadFunctionParameter("scope", true, false, false, null, BadScope.Prototype),
                new BadFunctionParameter(
                    "setLastAsReturn",
                    true,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("bool")
                )
            )
        );
        target.SetFunction(
            "CreateDefaultScope",
            ctx => ctx.Scope.GetRootScope().CreateChild("<customscope>", ctx.Scope, null),
            BadScope.Prototype
        );
        target.SetFunction("GetStackTrace", ctx => ctx.Scope.GetStackTrace(), BadNativeClassBuilder.GetNative("Array"));
        target.SetProperty("Native", MakeNative());
        target.SetFunction("GetArguments", GetArguments, BadNativeClassBuilder.GetNative("Array"));
        target.SetFunction<BadObject>("GetExtensionNames", GetExtensionNames, BadNativeClassBuilder.GetNative("Array"));
        target.SetFunction(
            "GetGlobalExtensionNames",
            GetGlobalExtensionNames,
            BadNativeClassBuilder.GetNative("Array")
        );
        target.SetFunction("GetTimeNow", GetTimeNow, BadNativeClassBuilder.GetNative("Table"));
        target.SetFunction<string>("ParseDate", ParseDate, BadNativeClassBuilder.GetNative("Table"));
        target.SetFunction(
            "GetNativeTypes",
            _ => new BadArray(BadNativeClassBuilder.NativeTypes.Cast<BadObject>().ToList()),
            BadNativeClassBuilder.GetNative("Array")
        );
        target.SetFunction(
            "GetRuntimeAssemblyPath",
            () =>
            {
                string path = Path.ChangeExtension(Assembly.GetEntryAssembly()!.Location, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "exe" : "");

                return path;
            },
            BadNativeClassBuilder.GetNative("string")
        );

        target.SetFunction<string, string>("Validate", ValidateSource, BadNativeClassBuilder.GetNative("Table"));
        target.SetFunction("NewGuid", () => Guid.NewGuid().ToString(), BadNativeClassBuilder.GetNative("string"));
        target.SetFunction<BadClass>("RegisterImportHandler", RegisterImportHandler, BadAnyPrototype.Instance);
        target.SetFunction<string>("IsApiRegistered", IsApiRegistered, BadNativeClassBuilder.GetNative("bool"));
    }

    private static BadObject IsApiRegistered(BadExecutionContext ctx, string api)
    {
        return ctx.Scope.RegisteredApis.Contains(api);
    }

    /// <summary>
    ///     Registers an Import Handler
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="cls">The Import Handler Class</param>
    /// <returns>Null</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Import Handler is invalid</exception>
    private static BadObject RegisterImportHandler(BadExecutionContext ctx, BadClass cls)
    {
        BadClassPrototype proto = cls.GetPrototype();
        if (!BadNativeClassBuilder.ImportHandler.IsSuperClassOf(proto))
        {
            throw BadRuntimeException.Create(ctx.Scope, "Invalid Import Handler");
        }

        BadModuleImporter? importer = ctx.Scope.GetSingleton<BadModuleImporter>();
        if (importer == null)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Module Importer not found");
        }

        BadInteropImportHandler handler = new BadInteropImportHandler(ctx, cls);
        importer.AddHandler(handler);

        return BadObject.Null;
    }

    /// <summary>
    ///     Validates a source string
    /// </summary>
    /// <param name="source">The Source String</param>
    /// <param name="file">The File Name</param>
    /// <returns>Validation Result</returns>
    private static BadObject ValidateSource(string source, string file)
    {
        BadExpressionValidatorContext result =
            BadExpressionValidatorContext.Validate(BadSourceParser.Parse(file, source));
        BadTable ret = new BadTable();
        ret.SetProperty("IsError", result.IsError);
        ret.SetProperty(
            "Messages",
            new BadArray(
                result.Messages.Select(
                        x =>
                        {
                            BadTable msg = new BadTable();
                            msg.SetProperty("Message", x.Message);
                            msg.SetProperty("Validator", x.Validator.ToString());
                            msg.SetProperty("Type", x.Type.ToString());
                            msg.SetProperty("Position", x.Expression.Position.ToString());

                            return (BadObject)msg;
                        }
                    )
                    .ToList()
            )
        );
        ret.SetFunction("GetMessageString", () => result.ToString(), BadNativeClassBuilder.GetNative("string"));

        return ret;
    }

    /// <summary>
    ///     Parses a date string
    /// </summary>
    /// <param name="date">The Date String</param>
    /// <returns>Bad Table with the parsed date</returns>
    private static BadObject ParseDate(string date)
    {
        DateTimeOffset d = DateTimeOffset.Parse(date);

        return GetDateTime(d);
    }

    /// <summary>
    ///     Converts a DateTimeOffset to a Bad Table
    /// </summary>
    /// <param name="time">The DateTimeOffset</param>
    /// <returns>Bad Table with the given DateTimeOffset</returns>
    private static BadObject GetDateTime(DateTimeOffset time)
    {
        BadTable table = new BadTable();
        table.SetProperty("Year", time.Year);
        table.SetProperty("Month", time.Month);
        table.SetProperty("Day", time.Day);
        table.SetProperty("Hour", time.Hour);
        table.SetProperty("Minute", time.Minute);
        table.SetProperty("Second", time.Second);
        table.SetProperty("Millisecond", time.Millisecond);
        table.SetProperty("UnixTimeMilliseconds", time.ToUnixTimeMilliseconds());
        table.SetProperty("UnixTimeSeconds", time.ToUnixTimeSeconds());
        table.SetProperty("Offset", time.Offset.ToString());

        table.SetProperty(
            "ToShortTimeString",
            new BadInteropFunction(
                "ToShortTimeString",
                args => CreateDate(
                        table,
                        args.Length < 1 ? null : ((IBadString)args[0]).Value
                    )
                    .ToShortTimeString(),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter(
                    "timeZone",
                    true,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
        table.SetProperty(
            "ToShortDateString",
            new BadInteropFunction(
                "ToShortDateString",
                args => CreateDate(
                        table,
                        args.Length < 1 ? null : ((IBadString)args[0]).Value
                    )
                    .ToShortDateString(),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter(
                    "timeZone",
                    true,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
        table.SetProperty(
            "ToLongTimeString",
            new BadInteropFunction(
                "ToLongTimeString",
                args => CreateDate(
                        table,
                        args.Length < 1 ? null : ((IBadString)args[0]).Value
                    )
                    .ToLongTimeString(),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter(
                    "timeZone",
                    true,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
        table.SetProperty(
            "ToLongDateString",
            new BadInteropFunction(
                "ToLongDateString",
                args => CreateDate(
                        table,
                        args.Length < 1 ? null : ((IBadString)args[0]).Value
                    )
                    .ToLongDateString(),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter(
                    "timeZone",
                    true,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );
        table.SetProperty(
            "Format",
            new BadInteropFunction(
                "Format",
                args => CreateDate(
                        table,
                        args.Length < 2 ? null : ((IBadString)args[1]).Value
                    )
                    .ToString(((IBadString)args[0]).Value),
                false,
                BadNativeClassBuilder.GetNative("string"),
                new BadFunctionParameter(
                    "format",
                    false,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                ),
                new BadFunctionParameter(
                    "timeZone",
                    true,
                    false,
                    false,
                    null,
                    BadNativeClassBuilder.GetNative("string")
                )
            )
        );

        return table;
    }

    /// <summary>
    ///     Creates a DateTime from a BadTable
    /// </summary>
    /// <param name="dateTable">The BadTable</param>
    /// <param name="timeZone">The TimeZone to use</param>
    /// <returns>The DateTime</returns>
    private static DateTime CreateDate(BadTable dateTable, string? timeZone = null)
    {
        //Convert Year, Month,Day, Hour,Minute, Second, Millisecond from IBadNumber to int
        int year = (int)((IBadNumber)dateTable.InnerTable["Year"]).Value;
        int month = (int)((IBadNumber)dateTable.InnerTable["Month"]).Value;
        int day = (int)((IBadNumber)dateTable.InnerTable["Day"]).Value;
        int hour = (int)((IBadNumber)dateTable.InnerTable["Hour"]).Value;
        int minute = (int)((IBadNumber)dateTable.InnerTable["Minute"]).Value;
        int second = (int)((IBadNumber)dateTable.InnerTable["Second"]).Value;
        int millisecond = (int)((IBadNumber)dateTable.InnerTable["Millisecond"]).Value;
        string offset = ((IBadString)dateTable.InnerTable["Offset"]).Value;

        //Create Date Time from the given values
        DateTimeOffset dtOffset =
            new DateTimeOffset(year, month, day, hour, minute, second, millisecond, TimeSpan.Parse(offset));

        DateTime dateTime = dtOffset.DateTime;

        if (timeZone != null)
        {
            dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, timeZone);
        }

        return dateTime;
    }

    /// <summary>
    ///     Returns the Current Time
    /// </summary>
    /// <returns>Bad Table with the Current Time</returns>
    private static BadObject GetTimeNow()
    {
        return GetDateTime(DateTime.Now);
    }

    /// <summary>
    ///     Returns all Extension Names of the given object
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="o">Object</param>
    /// <returns>Array of Extension Names</returns>
    private static BadObject GetExtensionNames(BadExecutionContext ctx, BadObject o)
    {
        return new BadArray(ctx.Scope.Provider.GetExtensionNames(o).ToList());
    }

    /// <summary>
    ///     Lists all global extension names
    /// </summary>
    /// <returns>Array of Extension Names</returns>
    private static BadObject GetGlobalExtensionNames(BadExecutionContext ctx)
    {
        return new BadArray(ctx.Scope.Provider.GetExtensionNames().ToList());
    }

    /// <summary>
    ///     Returns the arguments passed to the script
    /// </summary>
    /// <returns>Array of Arguments</returns>
    private static BadObject GetArguments()
    {
        return StartupArguments == null ? new BadArray() : new BadArray(StartupArguments.Select(x => (BadObject)x).ToList());
    }


    /// <summary>
    ///     Evaluates a string
    /// </summary>
    /// <param name="caller">Caller Context</param>
    /// <param name="str">Source String</param>
    /// <param name="fileObj">The File Name</param>
    /// <param name="optimizeExpr">Boolean that determines if the source will be optimized before execution</param>
    /// <param name="scope">The Scope that the source will be executed in</param>
    /// <param name="setLastAsReturn">
    ///     If true, the last result of the Source Enumeration will be set as the Task Result,
    ///     otherwise the exported variables are returned
    /// </param>
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

        string file = BadFileSystem.Instance.GetFullPath("<eval>");

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


        bool optimizeConstantFolding =
            BadNativeOptimizationSettings.Instance.UseConstantFoldingOptimization && optimizeE.Value;
        bool optimizeConstantSubstitution =
            BadNativeOptimizationSettings.Instance.UseConstantSubstitutionOptimization && optimizeE.Value;

        BadExecutionContext ctx;
        BadRuntime? runtime = caller.Scope.GetSingleton<BadRuntime>();
        if (runtime != null && (scope == BadObject.Null || scope is not BadScope))
        {
            // If the runtime is available, we can use the context builder to create a new context.
            // This is cleaner and also works better with the use of the Export Expressions.
            ctx = runtime.CreateContext(Path.GetDirectoryName(file) ?? "/");
        }
        else
        {
            // The Old way of doing it. This was a hack initially to ensure the configured interop apis are available in the scope,
            // even if we dont have a way to access the context builder.
            ctx =
                scope == BadObject.Null || scope is not BadScope sc
                    ? new BadExecutionContext(
                        caller.Scope.GetRootScope()
                            .CreateChild("<EvaluateAsync>", caller.Scope, null, BadScopeFlags.CaptureThrow)
                    )
                    : new BadExecutionContext(sc);
        }

        IEnumerable<BadExpression> exprs = BadSourceParser.Create(file, src.Value).Parse();

        if (optimizeConstantFolding)
        {
            exprs = BadConstantFoldingOptimizer.Optimize(exprs);
        }

        if (optimizeConstantSubstitution)
        {
            exprs = BadConstantSubstitutionOptimizer.Optimize(exprs);
        }

        BadTask task = null!;

        IEnumerable<BadObject> executor;
        if (setLastAsReturnB.Value) //If we want the last object as the return value, we just execute
        {
            executor = ctx.Execute(exprs);
        }
        else //Otherwise we return the exported variables of the script
        {
            executor = ExecuteScriptWithExports(ctx, exprs);
        }

        task = new BadTask(
            new BadInteropRunnable(

                // ReSharper disable once AccessToModifiedClosure
                SafeExecute(executor, ctx, () => task).GetEnumerator(),
                true
            ),
            "EvaluateAsync"
        );

        return task;
    }

    /// <summary>
    ///     Executes a script and returns the exported variables
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="exprs">The Expressions to execute</param>
    /// <returns>The exported variables</returns>
    private IEnumerable<BadObject> ExecuteScriptWithExports(BadExecutionContext ctx, IEnumerable<BadExpression> exprs)
    {
        foreach (BadObject o in ctx.Execute(exprs))
        {
            yield return o;
        }

        yield return ctx.Scope.GetExports();
    }

    /// <summary>
    ///     Wrapper that will execute a script and catch any errors that occur
    /// </summary>
    /// <param name="script">The Script to execute</param>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="getTask">The Task Getter</param>
    /// <returns></returns>
    private static IEnumerable<BadObject> SafeExecute(
        IEnumerable<BadObject> script,
        BadExecutionContext ctx,
        Func<BadTask> getTask)
    {
        foreach (BadObject o in script)
        {
            if (ctx.Scope.IsError)
            {
                getTask().Runnable.SetError(ctx.Scope.Error!);
            }

            yield return o;
        }
    }
}