namespace BadScript2.Runtime.Objects.Native
{
    /// <summary>
    ///     Defines properties for Native Types
    /// </summary>
    public interface IBadNative : IEquatable<IBadNative>
    {
        /// <summary>
        ///     The Value of the Native Object
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     The Type of the Native Object
        /// </summary>
        Type Type { get; }
    }
}