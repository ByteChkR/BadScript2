namespace BadScript2.IO.Virtual
{
    /// <summary>
    ///     Implements a special memory stream that can be used to read and write to a virtual file
    /// </summary>
    internal class BadVirtualFileStream : MemoryStream
    {
        /// <summary>
        ///     On Dispose Event
        /// </summary>
        public event Action? OnDispose;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            OnDispose?.Invoke();
        }
    }
}