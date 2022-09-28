namespace BadScript2.Common.Logging
{
    /// <summary>
    ///     Public facing interface for a logger
    /// </summary>
    public static class BadLogger
    {
        /// <summary>
        ///     On Message Handler
        /// </summary>
        public static event Action<BadLog>? OnLog;

        /// <summary>
        ///     Writes a Log to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        private static void Write(BadLog message)
        {
            OnLog?.Invoke(message);
        }

        /// <summary>
        ///     Writes a Log to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        public static void Log(string message)
        {
            Write(new BadLog(message));
        }

        /// <summary>
        ///     Writes a Log to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="mask">The mask of the message</param>
        public static void Log(string message, BadLogMask mask)
        {
            Write(new BadLog(message, mask));
        }

        /// <summary>
        ///     Writes a Log to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="mask">The mask of the message</param>
        /// <param name="position">The source position of the message</param>
        public static void Log(string message, BadLogMask mask, BadSourcePosition position)
        {
            Write(new BadLog(message, mask, position));
        }

        /// <summary>
        ///     Writes a Warning to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        public static void Warn(string message)
        {
            Write(new BadLog(message, null, null, BadLogType.Warning));
        }

        /// <summary>
        ///     Writes a Warning to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="mask">The mask of the message</param>
        public static void Warn(string message, BadLogMask mask)
        {
            Write(new BadLog(message, mask, null, BadLogType.Warning));
        }

        /// <summary>
        ///     Writes a Warning to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="mask">The mask of the message</param>
        /// <param name="position">The source position of the message</param>
        public static void Warn(string message, BadLogMask mask, BadSourcePosition position)
        {
            Write(new BadLog(message, mask, position, BadLogType.Warning));
        }

        /// <summary>
        ///     Writes an Error to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        public static void Error(string message)
        {
            Write(new BadLog(message, null, null, BadLogType.Warning));
        }

        /// <summary>
        ///     Writes an Error to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="mask">The mask of the message</param>
        public static void Error(string message, BadLogMask mask)
        {
            Write(new BadLog(message, mask, null, BadLogType.Error));
        }

        /// <summary>
        ///     Writes an Error to the Message Handler
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="mask">The mask of the message</param>
        /// <param name="position">The source position of the message</param>
        public static void Error(string message, BadLogMask mask, BadSourcePosition position)
        {
            Write(new BadLog(message, mask, position, BadLogType.Error));
        }
    }
}