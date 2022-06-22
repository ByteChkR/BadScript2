namespace BadScript2.Runtime.Objects.Native;

public interface IBadNumber : IBadNative
{
    new decimal Value { get; }
}