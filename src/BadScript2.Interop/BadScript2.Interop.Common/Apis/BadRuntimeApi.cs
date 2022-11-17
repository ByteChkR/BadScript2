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

public class BadRuntimeApi : BadInteropApi
{
    private readonly Dictionary<string, BadObject> m_Exports = new Dictionary<string, BadObject>();

    public BadRuntimeApi() : base("Runtime") { }
    public static IEnumerable<string>? StartupArguments { get; set; }

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

        return table;
    }


    private BadObject Native_IsNative(BadObject arg)
    {
        return arg is IBadNative;
    }

    private BadObject Native_IsFunction(BadObject arg)
    {
        return arg is BadFunction;
    }

    private BadObject Native_IsTable(BadObject arg)
    {
        return arg is BadTable;
    }

    private BadObject Native_IsString(BadObject arg)
    {
        return arg is IBadString;
    }

    private BadObject Native_IsNumber(BadObject arg)
    {
        return arg is IBadNumber;
    }

    private BadObject Native_IsBoolean(BadObject arg)
    {
        return arg is IBadBoolean;
    }

    private BadObject Native_IsArray(BadObject arg)
    {
        return arg is BadArray;
    }

    private BadObject Native_IsEnumerable(BadObject arg)
    {
        return arg.HasProperty("GetEnumerator");
    }

    private BadObject Native_IsEnumerator(BadObject arg)
    {
        return arg.HasProperty("GetCurrent") && arg.HasProperty("MoveNext");
    }


    public override void Load(BadTable target)
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
                "src",
                new BadFunctionParameter("file", true, false, false),
                new BadFunctionParameter("optimize", true, false, false),
                new BadFunctionParameter("scope", true, false, false),
                new BadFunctionParameter("setLastAsReturn", true, false, false)
            )
        );
        target.SetFunction("CreateDefaultScope", ctx => ctx.Scope.GetRootScope().CreateChild("<customscope>", ctx.Scope, null));
        target.SetFunction("GetStackTrace", ctx => ctx.Scope.GetStackTrace());
        target.SetProperty("Native", MakeNative());
        target.SetFunction<string, BadObject>("Export", Export);
        target.SetFunction<string>("Import", Import);
        target.SetFunction<string>("HasPackage", HasPackage);
        target.SetFunction("GetArguments", GetArguments);
        target.SetFunction<BadObject>("GetExtensionNames", GetExtensionNames);
        target.SetFunction("GetGlobalExtensionNames", GetGlobalExtensionNames);
        target.SetFunction("GetTimeNow", GetTimeNow);
        target.SetFunction("GetNativeTypes", c => new BadArray(BadNativeClassBuilder.NativeTypes.Cast<BadObject>().ToList()));
    }


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

        return table;
    }

    private BadObject GetExtensionNames(BadObject o)
    {
        return new BadArray(BadInteropExtension.GetExtensionNames(o).ToList());
    }

    private BadObject GetGlobalExtensionNames()
    {
        return new BadArray(BadInteropExtension.GetExtensionNames().ToList());
    }

    private BadObject GetArguments()
    {
        return StartupArguments == null
            ? new BadArray()
            : new BadArray(StartupArguments.Select(x => (BadObject)x).ToList());
    }

    private void Export(string name, BadObject obj)
    {
        m_Exports.Add(name, obj);
    }

    private BadObject HasPackage(string name)
    {
        return m_Exports.ContainsKey(name);
    }

    private BadObject Import(string name)
    {
        return m_Exports[name];
    }

    private BadObject Evaluate(BadExecutionContext caller, BadObject str, BadObject fileObj, BadObject optimizeExpr, BadObject scope)
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
                ? new BadExecutionContext(caller.Scope.GetRootScope().CreateChild("<EvaluateAsync>", caller.Scope, null))
                : new BadExecutionContext(sc);

        IEnumerable<BadExpression> exprs = BadSourceParser.Create(file, src.Value).Parse();
        if (optimize)
        {
            exprs = BadExpressionOptimizer.Optimize(exprs);
        }

        return ctx.Run(exprs) ?? BadObject.Null;
    }

    private BadObject EvaluateAsync(BadExecutionContext caller, BadObject str, BadObject fileObj, BadObject optimizeExpr, BadObject scope, BadObject setLastAsReturn)
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
        if(setLastAsReturn is not IBadBoolean setLastAsReturnB)
        {
            throw new BadRuntimeException("Evaluate: Argument 'setLastAsReturn' is not a boolean");
        }


        bool optimize = BadNativeOptimizationSettings.Instance.UseConstantExpressionOptimization && optimizeE.Value;

        BadExecutionContext ctx =
            scope == BadObject.Null || scope is not BadScope sc
                ? new BadExecutionContext(caller.Scope.GetRootScope().CreateChild("<EvaluateAsync>", caller.Scope, null))
                : new BadExecutionContext(sc);

        IEnumerable<BadExpression> exprs = BadSourceParser.Create(file, src.Value).Parse();
        if (optimize)
        {
            exprs = BadExpressionOptimizer.Optimize(exprs);
        }

        
        return new BadTask(new BadInteropRunnable(ctx.Execute(exprs).GetEnumerator(), setLastAsReturnB.Value), "EvaluateAsync");
    }
}