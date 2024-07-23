namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     Represents a Generic Object
/// </summary>
public interface IBadGenericObject
{
    /// <summary>
    ///     Indicates if the Object was already resolved to a concrete type
    /// </summary>
    bool IsResolved { get; }
    /// <summary>
    ///     Indicates if the Object is a Generic Object
    /// </summary>
    bool IsGeneric { get; }
    /// <summary>
    ///     The Generic Name of the Object
    /// </summary>
    string GenericName { get; }
    /// <summary>
    ///     The Generic Parameters of the Object
    /// </summary>
    IReadOnlyCollection<string> GenericParameters { get; }
    /// <summary>
    ///     Resolves the Generic Object to a concrete type
    /// </summary>
    /// <param name="args">The Generic Arguments</param>
    /// <returns>The Concrete Type</returns>
    BadObject CreateGeneric(BadObject[] args);
}