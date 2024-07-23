using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Settings;
namespace BadScript2.Runtime.Module;

/// <summary>
///     The Class that manages the importing of modules
/// </summary>
public class BadModuleImporter
{
    /// <summary>
    ///     The List of Import Handlers
    /// </summary>
    private readonly List<BadImportHandler> m_Handlers = new List<BadImportHandler>();

    /// <summary>
    ///     The Module Store instance
    /// </summary>
    private readonly BadModuleStore m_Store;

    /// <summary>
    ///     Creates a new BadModuleImporter
    /// </summary>
    /// <param name="store">The Module Store</param>
    public BadModuleImporter(BadModuleStore store)
    {
        m_Store = store;
    }

    public BadModuleImporter Clone(bool onlyTransient = true)
    {
        BadModuleImporter importer = new BadModuleImporter(m_Store);
        if (onlyTransient)
        {
            importer.m_Handlers.AddRange(m_Handlers.Where(x => x.IsTransient()));
        }
        else
        {
            importer.m_Handlers.AddRange(m_Handlers);
        }

        return importer;
    }

    /// <summary>
    ///     Adds a new Import Handler to the Importer
    /// </summary>
    /// <param name="handler">The Handler</param>
    /// <returns>Self</returns>
    public BadModuleImporter AddHandler(BadImportHandler handler)
    {
        m_Handlers.Add(handler);

        return this;
    }

    /// <summary>
    ///     Imports a module from the specified path
    /// </summary>
    /// <param name="path">The Path</param>
    /// <returns>The Imported Module</returns>
    public IEnumerable<BadObject> Get(string path)
    {
        for (int i = m_Handlers.Count - 1; i >= 0; i--)
        {
            BadImportHandler handler = m_Handlers[i];
            string hash = handler.GetHash(path);
            if (BadModuleSettings.Instance.UseModuleCaching && m_Store.IsCached(hash))
            {
                yield return m_Store.Get(hash);

                yield break;
            }

            if (handler.Has(path))
            {
                IEnumerable<BadObject> result = handler.Get(path);
                BadObject r = BadObject.Null;
                foreach (BadObject o in result)
                {
                    r = o;

                    yield return r;
                }

                r = r.Dereference();

                if (!BadModuleSettings.Instance.AllowImportHandlerNullReturn && r == BadObject.Null)
                {
                    continue;
                }

                if (BadModuleSettings.Instance.UseModuleCaching && r != BadObject.Null)
                {
                    m_Store.Cache(hash, r);
                }

                yield return r;

                yield break;
            }
        }

        if (BadModuleSettings.Instance.ThrowOnModuleUnresolved)
        {
            throw new BadRuntimeException("Module " + path + " could not be resolved.");
        }

        yield return BadObject.Null;
    }
}