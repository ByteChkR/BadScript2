namespace BadScript2.Runtime.Objects.Native
{
    public interface IBadNative : IEquatable<IBadNative>
    {
        object Value { get; }
        Type Type { get; }
    }
}