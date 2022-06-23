namespace BadScript2.Runtime.Objects.Native;

public interface IBadString : IBadNative
{
    new string Value { get; }
}