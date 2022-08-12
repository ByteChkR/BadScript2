namespace BadScript2.IO.Virtual;

internal class BadVirtualFileStream : MemoryStream
{
    public event Action? OnDispose;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        OnDispose?.Invoke();
    }
}