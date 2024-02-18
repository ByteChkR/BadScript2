using BadScript2.Runtime.Objects;

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
            if (m_Store.IsCached(hash))
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
                }
                m_Store.Cache(hash, r);

                yield return r;
                yield break;
            }
        }

        yield return BadObject.Null;
    }
}