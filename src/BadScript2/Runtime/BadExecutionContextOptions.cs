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
    ///     List of APIs that are loaded in the context
    /// </summary>
    private readonly List<BadInteropApi> m_Apis = new List<BadInteropApi>();

    /// <summary>
    ///     List of Extensions that are loaded in the context
    /// </summary>
    private readonly List<BadInteropExtension> m_Extensions = new List<BadInteropExtension>();

    /// <summary>
    ///     Creates a new instance of the <see cref="BadExecutionContextOptions" /> class.
    /// </summary>
    /// <param name="apis">Apis that should be added.</param>
    public BadExecutionContextOptions(IEnumerable<BadInteropApi> apis)
    {
        m_Apis.AddRange(apis);
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="BadExecutionContextOptions" /> class.
    /// </summary>
    /// <param name="apis">Apis that should be added.</param>
    public BadExecutionContextOptions(params BadInteropApi[] apis) : this((IEnumerable<BadInteropApi>)apis) { }

    /// <summary>
    ///     Creates a new instance of the <see cref="BadExecutionContextOptions" /> class.
    /// </summary>
    /// <param name="apis">Apis that should be added.</param>
    /// <param name="extensions">Extensions that should be added.</param>
    public BadExecutionContextOptions(IEnumerable<BadInteropApi> apis,
                                      IEnumerable<BadInteropExtension> extensions) : this(apis)
    {
        m_Extensions.AddRange(extensions);
    }

    /// <summary>
    ///     An Empty <see cref="BadExecutionContextOptions" /> instance.
    /// </summary>
    public static BadExecutionContextOptions Empty => new BadExecutionContextOptions();

    /// <summary>
    ///     List of APIs that are loaded in the context
    /// </summary>
    public IEnumerable<BadInteropApi> Apis => m_Apis;

    /// <summary>
    ///     Adds a new Extension to the context.
    /// </summary>
    /// <typeparam name="T">The Extension Type.</typeparam>
    public void AddExtension<T>() where T : BadInteropExtension, new()
    {
        T t = new T();
        AddExtension(t);
    }

    /// <summary>
    ///     Adds new Extensions to the context.
    /// </summary>
    /// <param name="extensions">The Extensions to add.</param>
    public void AddExtensions(params BadInteropExtension[] extensions)
    {
        foreach (BadInteropExtension extension in extensions)
        {
            AddExtension(extension);
        }
    }

    /// <summary>
    ///     Adds a new Extension to the context.
    /// </summary>
    /// <param name="extension">The Extension to add.</param>
    public void AddExtension(BadInteropExtension extension)
    {
        m_Extensions.Add(extension);
    }

    /// <summary>
    ///     Adds a new Api to the context.
    /// </summary>
    /// <param name="api">The Api to add.</param>
    public void AddApi(BadInteropApi api)
    {
        m_Apis.Add(api);
    }

    /// <summary>
    ///     Adds or Replaces apis in the context.
    /// </summary>
    /// <param name="apis">The Apis to add or replace.</param>
    public void AddOrReplaceApis(IEnumerable<BadInteropApi> apis)
    {
        foreach (BadInteropApi api in apis)
        {
            AddOrReplaceApi(api);
        }
    }

    /// <summary>
    ///     Adds or Replaces an api in the context.
    /// </summary>
    /// <param name="api">The Api to add or replace.</param>
    public void AddOrReplaceApi(BadInteropApi api)
    {
        int index = m_Apis.FindIndex(x => x.Name == api.Name);

        if (index == -1)
        {
            AddApi(api);
        }
        else
        {
            m_Apis[index] = api;
        }
    }

    /// <summary>
    ///     Adds Apis to the context.
    /// </summary>
    /// <param name="apis">The Apis to add.</param>
    public void AddApis(IEnumerable<BadInteropApi> apis)
    {
        m_Apis.AddRange(apis);
    }

    /// <summary>
    ///     Builds a new <see cref="BadExecutionContext" /> with the options provided in this Options Instance.
    /// </summary>
    /// <returns>The new <see cref="BadExecutionContext" /></returns>
    public BadExecutionContext Build()
    {
        BadExecutionContext ctx = BadExecutionContext.Create(new BadInteropExtensionProvider(m_Extensions.ToArray()));

        foreach (BadInteropApi api in m_Apis)
        {
            BadTable table;

            if (ctx.Scope.HasLocal(api.Name, ctx.Scope) &&
                ctx.Scope.GetVariable(api.Name)
                   .Dereference(null) is BadTable t)
            {
                table = t;
            }
            else
            {
                table = new BadTable();
                ctx.Scope.DefineVariable(api.Name, table);
            }

            api.Load(ctx, table);
        }

        foreach (BadClassPrototype type in BadNativeClassBuilder.NativeTypes)
        {
            ctx.Scope.DefineVariable(type.Name, type);
        }

        return ctx;
    }

    /// <summary>
    ///     Clones this <see cref="BadExecutionContextOptions" /> instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public BadExecutionContextOptions Clone()
    {
        return new BadExecutionContextOptions(m_Apis, m_Extensions);
    }
}