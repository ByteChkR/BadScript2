using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Module;

/// <summary>
///     Is the store for all loaded modules
/// </summary>
public class BadModuleStore
{
    /// <summary>
    ///     The Internal Module Store
    /// </summary>
    private readonly Dictionary<string, BadObject> m_Modules = new Dictionary<string, BadObject>();

    /// <summary>
    ///     Returns true if the module is cached
    /// </summary>
    /// <param name="hash">The Unique Hash for the module</param>
    /// <returns>True if the module is cached</returns>
    public bool IsCached(string hash)
    {
        return m_Modules.ContainsKey(hash);
    }

    /// <summary>
    ///     Cache the specified module
    /// </summary>
    /// <param name="hash">The Unique Hash for the module</param>
    /// <param name="module">The Module</param>
    public void Cache(string hash, BadObject module)
    {
        m_Modules[hash] = module;
    }

    /// <summary>
    ///     Returns the module with the specified hash
    /// </summary>
    /// <param name="hash">The Unique Hash for the module</param>
    /// <returns>The Module</returns>
    public BadObject Get(string hash)
    {
        return m_Modules[hash];
    }
}