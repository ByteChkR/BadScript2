using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
///     Provides settings for creating a new <see cref="BadExecutionContext" />
/// </summary>
public class BadExecutionContextOptions
{
    /// <summary>
    ///     The Default Options.
    /// </summary>
    public static readonly BadExecutionContextOptions Default = new BadExecutionContextOptions();

    /// <summary>
    ///     List of APIs that are loaded in the context
    /// </summary>
    public readonly List<BadInteropApi> Apis = new List<BadInteropApi>();

    /// <summary>
    ///     Creates a new instance of the <see cref="BadExecutionContextOptions" /> class.
    /// </summary>
    /// <param name="apis">Apis that should be added.</param>
    public BadExecutionContextOptions(IEnumerable<BadInteropApi> apis)
    {
        Apis.AddRange(apis);
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="BadExecutionContextOptions" /> class.
    /// </summary>
    /// <param name="apis">Apis that should be added.</param>
    public BadExecutionContextOptions(params BadInteropApi[] apis) : this((IEnumerable<BadInteropApi>)apis) { }

    /// <summary>
    ///     Builds a new <see cref="BadExecutionContext" /> with the options provided in this Options Instance.
    /// </summary>
    /// <returns>The new <see cref="BadExecutionContext" /></returns>
    public BadExecutionContext Build()
    {
        BadExecutionContext ctx = BadExecutionContext.Create();

        foreach (BadInteropApi api in Apis)
        {
            //BadLogger.Log($"Registering API: {api.Name}", "ContextBuilder");
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
            //BadLogger.Log($"Adding Native Type {type.Name}", "ContextBuilder");
            ctx.Scope.DefineVariable(type.Name, type);
        }

        return ctx;
    }

    public BadExecutionContextOptions Clone()
    {
        return new BadExecutionContextOptions(Apis);
    }
}