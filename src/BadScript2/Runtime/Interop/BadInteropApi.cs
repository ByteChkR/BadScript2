using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop;

/// <summary>
///     Implements an Interop API for the BS2 Language
/// </summary>
public abstract class BadInteropApi
{
    /// <summary>
    ///     Creates a new Interop API
    /// </summary>
    /// <param name="name">Name of the API</param>
    protected BadInteropApi(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Name of the API
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Loads the API into the given Table
    /// </summary>
    /// <param name="target">Table Target</param>
    public abstract void Load(BadTable target);
}