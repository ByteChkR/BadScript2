using BadScript2.Debugger.Scriptable;
using BadScript2.Interop.Common;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.Compression;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Interop.Net;
using BadScript2.Parser;
using BadScript2.Parser.Operators;
using BadScript2.Reader;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;

internal class Program
{
    private static void InitializeBadScript(WebApplication app)
    {
        BadSettingsProvider.SetRootSettings(new BadSettings());
        BadNativeClassBuilder.AddNative(BadTask.Prototype);
        BadCommonInterop.AddExtensions();
        BadInteropExtension.AddExtension<BadScriptDebuggerExtension>();
        BadInteropExtension.AddExtension<BadNetInteropExtensions>();
        BadInteropExtension.AddExtension<BadLinqExtensions>();

        BadExecutionContextOptions.Default.Apis.AddRange(BadCommonInterop.Apis);
        BadExecutionContextOptions.Default.Apis.Add(new BadIOApi());
        BadExecutionContextOptions.Default.Apis.Add(new BadJsonApi());
        BadExecutionContextOptions.Default.Apis.Add(new BadNetApi());
        BadExecutionContextOptions.Default.Apis.Add(new BadCompressionApi());

        BadTable table = new BadTable();
        table.SetFunction<string, BadFunction>(
            "MapGet",
            (s, f) =>
            {
                OnRegister("GET", s, f);
                app.MapGet(s, c => Map(f, c));
            }
        );
        table.SetFunction<string, BadFunction>(
            "MapPut",
            (s, f) =>
            {
                OnRegister("PUT", s, f);
                app.MapPut(s, c => Map(f, c));
            }
        );
        table.SetFunction<string, BadFunction>(
            "MapPost",
            (s, f) =>
            {
                OnRegister("POST", s, f);
                app.MapPost(s, c => Map(f, c));
            }
        );

        BadExecutionContext ctx = BadExecutionContextOptions.Default.Build();
        ctx.Scope.DefineVariable("App", table);

        ctx.Run(new BadSourceParser(new BadSourceReader("./server.bs", File.ReadAllText("./server.bs")), BadOperatorTable.Instance).Parse());

        app.Run();
    }

    private static void OnRegister(string op, string pattern, BadFunction func)
    {
        Console.WriteLine("Registering {0} for {1}", op, pattern);
    }

    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        WebApplication app = builder.Build();

        InitializeBadScript(app);

        app.Run();
    }


    private static Task Map(BadFunction func, HttpContext context)
    {
        return Task.Run(
            async () =>
            {
                BadObject ret = BadObject.Null;
                foreach (BadObject o in func.Invoke(new BadObject[] { new BadReflectedObject(context) }, BadExecutionContext.Create()))
                {
                    ret = o;
                }

                ret = ret.Dereference();


                if (ret.CanUnwrap())
                {
                    await context.Response.WriteAsJsonAsync(ret.Unwrap());
                }
                else if (ret is BadReflectedObject ro)
                {
                    await context.Response.WriteAsJsonAsync(ro.Instance);
                }
                else
                {
                    await context.Response.WriteAsync(BadJson.ToJson(ret));
                }
            }
        );
    }
}