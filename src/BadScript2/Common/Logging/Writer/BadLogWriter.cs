namespace BadScript2.Common.Logging.Writer
{
    /// <summary>
    ///     Base Class of all log writers
    /// </summary>
    public abstract class BadLogWriter : IDisposable
    {
        public bool IsActive { get; private set; }

        /// <summary>
        ///     Implements the IDisposable interface
        /// </summary>
        public virtual void Dispose()
        {
            Unregister();
        }

        /// <summary>
        ///     Writes a log message to the log writer
        /// </summary>
        /// <param name="log">The log to be written</param>
        protected abstract void Write(BadLog log);

        /// <summary>
        ///     Inner Write Log Method makes sure the log is only written if the Mask Settings contain the log mask
        /// </summary>
        /// <param name="log">The Log to be written</param>
        private void InnerWrite(BadLog log)
        {
            if (BadLogWriterSettings.Instance.Mask.Contains(log.Mask))
            {
                Write(log);
            }
        }

        /// <summary>
        ///     Registers the Log Writer to the Log System
        /// </summary>
        public void Register()
        {
            if (IsActive)
            {
                return;
            }

            BadLogger.OnLog += InnerWrite;
            IsActive = true;
        }


        /// <summary>
        ///     Unregisters the Log Writer from the Log System
        /// </summary>
        public void Unregister()
        {
            if (!IsActive)
            {
                return;
            }

            BadLogger.OnLog -= InnerWrite;
            IsActive = false;
        }
    }
}