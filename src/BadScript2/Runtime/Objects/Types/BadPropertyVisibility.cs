namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     The Visibility of a Property
/// </summary>
[Flags]
public enum BadPropertyVisibility
{
    /// <summary>
    ///     Public, Visible to all
    /// </summary>
    Public = 1,

    /// <summary>
    ///     Protected, Visible to the Class and Subclasses
    /// </summary>
    Protected = 2,

    /// <summary>
    ///     Private, Visible only to the Class
    /// </summary>
    Private = 4,

    /// <summary>
    ///     All
    /// </summary>
    All = Public | Protected | Private,
}