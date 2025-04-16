using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using BadScript2.Common;
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
[BadInteropApi("Runtime")]
internal partial class BadRuntimeApi
{
    /// <summary>
    ///     The Startup Arguments that were passed to the Runtime
    /// </summary>
    public static IEnumerable<string>? StartupArguments { get; set; }

    /// <inheritdoc/>
    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty("Native", MakeNative());
    }

    /// <summary>
    ///     Creates the "Native" Table
    /// </summary>
    /// <returns>Bad Table</returns>
    private BadTable MakeNative()
    {
        BadTable table = new BadTable();

        new BadNativeApi().LoadRawApi(table);

        return table;
    }

    /// <summary>
    /// Creates a default scope based off of the root scope of the caller
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <returns>The created scope</returns>
    [BadMethod(description: "Creates a default scope based off of the root scope of the caller")]
    [return: BadReturn("The created scope")]
    private BadScope CreateDefaultScope(BadExecutionContext ctx)
    {
        return ctx.Scope.GetRootScope()
                  .CreateChild("<customscope>", ctx.Scope, null);
    }

    /// <summary>
    /// Evaluates a BadScript Source String
    /// </summary>
    /// <param name="caller">The Caller Context</param>
    /// <param name="source">The Source of the Script</param>
    /// <param name="file">An (optional but recommended) file path, it will be used to determine the working directory of the script.</param>
    /// <param name="optimize">If true, any optimizations that are activated in the settings will be applied.</param>
    /// <param name="scope">An (optional) scope that the execution takes place in, if not specified, an Instance of BadRuntime will get searched and a scope will be created from it, if its not found, a scope will be created from the root scope of the caller.</param>
    /// <param name="setLastAsReturn">If true, the last element that was returned from the enumeration will be the result of the task. Otherwise the result will be the exported objects of the script.</param>
    /// <returns>An Awaitable Task</returns>
    [BadMethod(description: "Evaluates a BadScript Source String")]
    [return: BadReturn("An Awaitable Task")]
    private BadTask EvaluateAsync(BadExecutionContext caller,
                                  [BadParameter(description: "The Source of the Script")]
                                  string source,
                                  [BadParameter(description:
                                                   "An (optional but recommended) file path, it will be used to determine the working directory of the script."
                                               )]
                                  string? file = null,
                                  [BadParameter(description:
                                                   "If true, any optimizations that are activated in the settings will be applied."
                                               )]
                                  bool optimize = true,
                                  [BadParameter(description:
                                                   "An (optional) scope that the execution takes place in, if not specified, an Instance of BadRuntime will get searched and a scope will be created from it, if its not found, a scope will be created from the root scope of the caller."
                                               )]
                                  BadScope? scope = null,
                                  [BadParameter(description:
                                                   "If true, the last element that was returned from the enumeration will be the result of the task. Otherwise the result will be the exported objects of the script."
                                               )]
                                  bool setLastAsReturn = false)
    {
        file = BadFileSystem.Instance.GetFullPath(file ?? "<eval>");

        bool optimizeConstantFolding =
            BadNativeOptimizationSettings.Instance.UseConstantFoldingOptimization && optimize;

        bool optimizeConstantSubstitution =
            BadNativeOptimizationSettings.Instance.UseConstantSubstitutionOptimization && optimize;

        BadExecutionContext ctx;
        BadRuntime? runtime = caller.Scope.GetSingleton<BadRuntime>();

        if (runtime != null && scope == null)
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
                scope == null
                    ? new BadExecutionContext(caller.Scope.GetRootScope()
                                                    .CreateChild("<EvaluateAsync>",
                                                                 caller.Scope,
                                                                 null,
                                                                 BadScopeFlags.CaptureThrow
                                                                )
                                             )
                    : new BadExecutionContext(scope);
        }

        IEnumerable<BadExpression> exprs = BadSourceParser.Create(file, source)
                                                          .Parse();

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

        if (setLastAsReturn) //If we want the last object as the return value, we just execute
        {
            executor = ctx.Execute(exprs);
        }
        else //Otherwise we return the exported variables of the script
        {
            executor = ExecuteScriptWithExports(ctx, exprs);
        }

        task = new BadTask(new BadInteropRunnable(
                                                  // ReSharper disable once AccessToModifiedClosure
                                                  SafeExecute(executor, ctx, () => task)
                                                      .GetEnumerator(),
                                                  true
                                                 ),
                           "EvaluateAsync"
                          );

        return task;
    }

    /// <summary>
    /// Returns the Stack Trace of the current Execution Context
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <returns>The Stack Trace</returns>
    [BadMethod(description: "Returns the Stack Trace of the current Execution Context")]
    [return: BadReturn("The Stack Trace")]
    private string GetStackTrace(BadExecutionContext ctx)
    {
        return ctx.Scope.GetStackTrace();
    }


    /// <summary>
    /// Will return the Root Scope of the given Execution Context
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <returns>The Root Scope</returns>
    [BadMethod]
    private BadScope GetRootScope(BadExecutionContext ctx)
    {
        return ctx.Scope.GetRootScope();
    }


    /// <summary>
    /// Will return the Root Scope of the given Execution Context
    /// </summary>
    /// <returns>The Root Scope</returns>
    [BadMethod(description: "Returns all Native Types")]
    [return: BadReturn("An Array containing all Native Types")]
    private BadArray GetNativeTypes()
    {
        return new BadArray(BadNativeClassBuilder.NativeTypes.Cast<BadObject>()
                                                 .ToList()
                           );
    }

    /// <summary>
    /// Will return the Assembly Path of the current Runtime
    /// </summary>
    /// <returns>The Assembly Path</returns>
    [BadMethod(description: "Returns the Assembly Path of the current Runtime")]
    [return: BadReturn("The Assembly Path")]
    private string GetRuntimeAssemblyPath()
    {
        string path = Path.ChangeExtension(Assembly.GetEntryAssembly()!.Location,
                                           RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "exe" : ""
                                          );

        return path;
    }

    /// <summary>
    /// Returns a new Guid
    /// </summary>
    /// <returns>A new Guid</returns>
    [BadMethod(description: "Returns a new Guid")]
    [return: BadReturn("A new Guid")]
    private string NewGuid()
    {
        return Guid.NewGuid()
                   .ToString();
    }

    /// <summary>
    /// Returns true if an api with that name is registered
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="api">The Api Name</param>
    /// <returns>True if the api is registered</returns>
    [BadMethod(description: "Returns true if an api with that name is registered")]
    [return: BadReturn("True if the api is registered")]
    private BadObject IsApiRegistered(BadExecutionContext ctx, [BadParameter(description: "The Api Name")] string api)
    {
        return ctx.Scope.RegisteredApis.Contains(api);
    }

    /// <summary>
    /// Creates a new Reference Object
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="refText">The Text for the Reference</param>
    /// <param name="get">The Getter Function</param>
    /// <param name="set">The Setter Function</param>
    /// <param name="delete">The Delete Function</param>
    /// <returns>The Reference Object</returns>
    [BadMethod(description: "Creates a new Reference Object")]
    [return: BadReturn("The Reference Object")]
    private BadObject MakeReference(BadExecutionContext ctx,
                                    [BadParameter(description: "The Text for the Reference")]
                                    string refText,
                                    [BadParameter(description: "The getter Function")]
                                    BadFunction get,
                                    [BadParameter(description: "The setter Function")]
                                    BadFunction? set = null,
                                    [BadParameter(description: "The delete Function")]
                                    BadFunction? delete = null)
    {
        if (set == null && delete == null)
        {
            return BadObjectReference.Make(refText, (p) => GetReferenceValue(ctx, get));
        }

        if (delete == null && set != null)
        {
            return BadObjectReference.Make(refText,
                                           (p) => GetReferenceValue(ctx, get),
                                           (value, _, _) => SetReferenceValue(ctx, set, value)
                                          );
        }

        if (delete != null && set == null)
        {
            return BadObjectReference.Make(refText,
                                           (p) => GetReferenceValue(ctx, get),
                                           (Action<BadObject, BadSourcePosition?, BadPropertyInfo?>?)null,
                                           (p) => SetReferenceValue(ctx, delete, BadObject.Null)
                                          );
        }

        return BadObjectReference.Make(refText,
                                       (p) => GetReferenceValue(ctx, get),
                                       (value, _, _) => SetReferenceValue(ctx, set!, value),
                                       (p) => DeleteReference(ctx, delete!)
                                      );
    }

    /// <summary>
    /// Calls the Delete Function of the Reference
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="func">The Delete Function</param>
    private void DeleteReference(BadExecutionContext ctx, BadFunction func)
    {
        foreach (BadObject? o in func.Invoke(Array.Empty<BadObject>(), ctx)) { }
    }

    /// <summary>
    /// Calls the Getter Function of the Reference
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="func">The Getter Function</param>
    /// <returns>The Value of the Reference</returns>
    private BadObject GetReferenceValue(BadExecutionContext ctx, BadFunction func)
    {
        BadObject? result = BadObject.Null;

        foreach (BadObject? o in func.Invoke(Array.Empty<BadObject>(), ctx))
        {
            result = o;
        }

        return result.Dereference(null);
    }

    /// <summary>
    /// Calls the Setter Function of the Reference
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="func">The Setter Function</param>
    /// <param name="value">The Value to set</param>
    private void SetReferenceValue(BadExecutionContext ctx, BadFunction func, BadObject value)
    {
        foreach (BadObject? o in func.Invoke(new[] { value },
                                             ctx
                                            )) { }
    }

    /// <summary>
    /// Returns all registered apis
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <returns>An Array containing all registered apis</returns>
    [BadMethod(description: "Returns all registered apis")]
    [return: BadReturn("An Array containing all registered apis")]
    private BadArray GetRegisteredApis(BadExecutionContext ctx)
    {
        return new BadArray(ctx.Scope.RegisteredApis.Select(x => (BadObject)x)
                               .ToList()
                           );
    }

    /// <summary>
    /// Imports a module at runtime
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="path">The Path to the Module</param>
    /// <returns>An Awaitable Task</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Module Importer is not found</exception>
    [BadMethod("ImportAsync", "Imports a module at runtime.")]
    private BadTask ImportModuleAsync(BadExecutionContext ctx, string path)
    {
        BadModuleImporter? importer = ctx.Scope.GetSingleton<BadModuleImporter>();
        
        if (importer == null)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Module Importer not found");
        }
        
        
        BadTask task = null!;

        IEnumerable<BadObject> executor = importer.Get(path);

        task = new BadTask(new BadInteropRunnable(
                // ReSharper disable once AccessToModifiedClosure
                SafeExecute(executor, ctx, () => task)
                    .GetEnumerator(),
                true
            ),
            "EvaluateAsync"
        );

        return task;
        
    }

    /// <summary>
    ///     Registers an Import Handler
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="cls">The Import Handler Class</param>
    /// <returns>Null</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Import Handler is invalid</exception>
    [BadMethod(description: "Registers an Import Handler")]
    private static void RegisterImportHandler(BadExecutionContext ctx,
                                              [BadParameter(description:
                                                               "An Import handler implementing the IImportHandler Interface",
                                                               nativeType: "IImportHandler"
                                                           )]
                                              BadClass cls)
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
    }

    /// <summary>
    ///     Validates a source string
    /// </summary>
    /// <param name="source">The Source String</param>
    /// <param name="file">The File Name</param>
    /// <returns>Validation Result</returns>
    [BadMethod("Validate", "Validates a source string")]
    [return: BadReturn("Validation Result")]
    private static BadTable ValidateSource([BadParameter(description: "The Source to Validate")] string source,
                                           [BadParameter(description: "The File Name")] string file)
    {
        BadExpressionValidatorContext result =
            BadExpressionValidatorContext.Validate(BadSourceParser.Parse(file, source));
        BadTable ret = new BadTable();
        ret.SetProperty("IsError", result.IsError);

        ret.SetProperty("Messages",
                        new BadArray(result.Messages.Select(x =>
                                                            {
                                                                BadTable msg = new BadTable();
                                                                msg.SetProperty("Message", x.Message);
                                                                msg.SetProperty("Validator", x.Validator.ToString());
                                                                msg.SetProperty("Type", x.Type.ToString());

                                                                msg.SetProperty("Position",
                                                                                x.Expression.Position.ToString()
                                                                               );

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
    [BadMethod(description: "Parses a date string")]
    [return: BadReturn("Bad Table with the parsed date")]
    private static BadObject ParseDate([BadParameter(description: "The date string")] string date)
    {
        DateTimeOffset d = DateTimeOffset.Parse(date);

        return new BadDate(d);
    }

    /// <summary>
    ///     Returns the Current Time
    /// </summary>
    /// <returns>Bad Table with the Current Time</returns>
    [BadMethod(description: "Returns the Current Time")]
    [return: BadReturn("The Current Time")]
    private static BadObject GetTimeNow()
    {
        return BadDate.Now;
    }

    /// <summary>
    ///     Returns all Extension Names of the given object
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="o">Object</param>
    /// <returns>Array of Extension Names</returns>
    [BadMethod(description: "Lists all extension names of the given object")]
    [return: BadReturn("An Array containing all Extension Names")]
    private static BadArray GetExtensionNames(BadExecutionContext ctx,
        [BadParameter(description: "Object")] BadObject o)
    {
        return new BadArray(ctx.Scope.Provider.GetExtensionNames(o)
            .ToList()
        );
    }
    /// <summary>
    ///     Returns all Extensions of the given object(will create the objects based on the supplied object)
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="o">Object</param>
    /// <returns>Array of Extension Names</returns>
    [BadMethod(description: "Returns all Extensions of the given object(will create the objects based on the supplied object)")]
    [return: BadReturn("A Table containing all Extensions")]
    private static BadTable GetExtensions(BadExecutionContext ctx,
        [BadParameter(description: "Object")] BadObject o)
    {
        return ctx.Scope.Provider.GetExtensions(o);
    }

    /// <summary>
    ///     Lists all global extension names
    /// </summary>
    /// <returns>Array of Extension Names</returns>
    [BadMethod(description: "Lists all global extension names")]
    [return: BadReturn("An Array containing all Extension Names")]
    private static BadObject GetGlobalExtensionNames(BadExecutionContext ctx)
    {
        return new BadArray(ctx.Scope.Provider.GetExtensionNames()
                               .ToList()
                           );
    }

    /// <summary>
    ///     Returns the arguments passed to the script
    /// </summary>
    /// <returns>Array of Arguments</returns>
    [BadMethod(description: "Gets the Commandline Arguments")]
    [return: BadReturn("An Array containing all Commandline Arguments")]
    private static BadArray GetArguments()
    {
        return StartupArguments == null
                   ? new BadArray()
                   : new BadArray(StartupArguments.Select(x => (BadObject)x)
                                                  .ToList()
                                 );
    }


    /// <summary>
    /// Gets the Attributes of the given objects members
    /// </summary>
    /// <param name="obj">The Object to get the members from</param>
    /// <returns>A Table containing the Attributes of the given objects members</returns>
    [BadMethod(description: "Gets the Attributes of the given objects members")]
    [return: BadReturn("A Table containing the Attributes of the given objects members.")]
    private static BadArray GetMembers(BadObject obj)
    {
        if (obj is BadClass cls)
        {
            return cls.Scope.GetMemberInfos();
        }

        return new BadArray();
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
    private static IEnumerable<BadObject> SafeExecute(IEnumerable<BadObject> script,
                                                      BadExecutionContext ctx,
                                                      Func<BadTask> getTask)
    {
        using IEnumerator<BadObject>? enumerator = script.GetEnumerator();

        while (true)
        {
            try
            {
                if (!enumerator.MoveNext())
                {
                    break;
                }
            }
            catch (BadRuntimeErrorException e)
            {
                getTask()
                    .Runnable.SetError(e.Error);

                break;
            }
            catch (Exception e)
            {
                getTask()
                    .Runnable.SetError(BadRuntimeError.FromException(e, ctx.Scope.GetStackTrace()));

                break;
            }

            yield return enumerator.Current ?? BadObject.Null;
        }
    }
}