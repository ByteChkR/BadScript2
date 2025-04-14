namespace BadScript2.IO;

/// <summary>
///     The Write Modes of the File System Abstraction
/// </summary>
public enum BadWriteMode
{
    /// <summary>
    /// Creates a new file or overwrites an existing file
    /// </summary>
    CreateNew,
    /// <summary>
    /// Creates a new file or appends to an existing file
    /// </summary>
    Append,
}