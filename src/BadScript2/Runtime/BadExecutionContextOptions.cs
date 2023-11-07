using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime;

/// <summary>
///     Provides settings for creating a new <see cref="BadExecutionContext" />
/// </summary>
public class BadExecutionContextOptions
{
    public static BadExecutionContextOptions Empty => new BadExecutionContextOptions();
    public void AddExtension<T>() where T : BadInteropExtension, new()
    {
        T t = new T();
        AddExtension(t);
    }

    public void AddExtensions(params BadInteropExtension[] extensions)
    {
        foreach (BadInteropExtension extension in extensions)
        {
            AddExtension(extension);
        }
    }
    public void AddExtension(BadInteropExtension extension)
    {
        m_Extensions.Add(extension);
    }
	/// <summary>
	///     List of APIs that are loaded in the context
	/// </summary>
	private readonly List<BadInteropApi> m_Apis = new List<BadInteropApi>();
    
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

    public BadExecutionContextOptions(IEnumerable<BadInteropApi> apis, IEnumerable<BadInteropExtension> extensions) : this(apis)
    {
        m_Extensions.AddRange(extensions);
    }

	/// <summary>
	///     List of APIs that are loaded in the context
	/// </summary>
	public IEnumerable<BadInteropApi> Apis => m_Apis;

    public void AddApi(BadInteropApi api)
    {
        m_Apis.Add(api);
    }

    public void AddOrReplaceApis(IEnumerable<BadInteropApi> apis)
    {
        foreach (BadInteropApi api in apis)
        {
            AddOrReplaceApi(api);
        }
    }

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

            if (ctx.Scope.HasLocal(api.Name, ctx.Scope) && ctx.Scope.GetVariable(api.Name).Dereference() is BadTable t)
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
            ctx.Scope.DefineVariable(type.Name, type);
        }


        return ctx;
    }

    public BadExecutionContextOptions Clone()
    {
        return new BadExecutionContextOptions(m_Apis, m_Extensions);
    }
}