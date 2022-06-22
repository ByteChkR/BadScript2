using BadScript2.Common.Logging;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

public class BadExecutionContextOptions
{
    public static readonly BadExecutionContextOptions Default = new BadExecutionContextOptions();

    public readonly List<BadInteropApi> Apis = new List<BadInteropApi>();

    public BadExecutionContextOptions(IEnumerable<BadInteropApi> apis)
    {
        Apis.AddRange(apis);
    }

    public BadExecutionContextOptions(params BadInteropApi[] apis) : this((IEnumerable<BadInteropApi>)apis) { }

    public BadExecutionContext Build()
    {
        BadExecutionContext ctx = BadExecutionContext.Create();

        foreach (BadInteropApi api in Apis)
        {
            BadLogger.Log($"Registering API: {api.Name}", "ContextBuilder");
            BadTable table;
            if (ctx.Scope.HasLocal(api.Name) && ctx.Scope.GetVariable(api.Name).Dereference() is BadTable t)
            {
                table = t;
            }
            else
            {
                table = new BadTable();
                ctx.Scope.DefineVariable(api.Name, table);
            }

            api.Load(table);
        }

        foreach (BadClassPrototype type in BadNativeClassBuilder.NativeTypes)
        {
            BadLogger.Log($"Adding Native Type {type.Name}", "ContextBuilder");
            ctx.Scope.DefineVariable(type.Name, type);
        }

        return ctx;
    }
}