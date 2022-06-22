namespace BadScript2.Common.Logging.Writer
{
    public abstract class BadLogWriter : IDisposable
    {
        public virtual void Dispose()
        {
            Unregister();
        }

        protected abstract void Write(BadLog log);

        private void InnerWrite(BadLog log)
        {
            if (BadLogWriterSettings.Instance.Mask.Contains(log.Mask))
            {
                Write(log);
            }
        }

        public void Register()
        {
            BadLogger.OnLog += InnerWrite;
        }

        public void Unregister()
        {
            BadLogger.OnLog -= InnerWrite;
        }
    }
}