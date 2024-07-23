using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime.Module;

/// <summary>
///     Defines the shape of the import handler for the module importer
/// </summary>
public abstract class BadImportHandler
{
    public abstract bool IsTransient();

    /// <summary>
    ///     Returns true if the specified path is available
    /// </summary>
    /// <param name="path">The Path</param>
    /// <returns>true if the path is available</returns>
    public abstract bool Has(string path);

    /// <summary>
    ///     Returns a unique hash for the specified path
    /// </summary>
    /// <param name="path">The Path</param>
    /// <returns>The Unique Hash</returns>
    public abstract string GetHash(string path);

    /// <summary>
    ///     Imports a module from the specified path
    /// </summary>
    /// <param name="path">The Path</param>
    /// <returns>The Imported Module</returns>
    public abstract IEnumerable<BadObject> Get(string path);
}