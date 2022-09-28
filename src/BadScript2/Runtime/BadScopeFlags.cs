namespace BadScript2.Runtime
{
    /// <summary>
    ///     Defines Different Behaviours for the Current Scope
    /// </summary>
    [Flags]
    public enum BadScopeFlags
    {
        /// <summary>
        ///     No Special Behaviour
        /// </summary>
        None = 0,

        /// <summary>
        ///     Allows the usage of the Return Keyword
        /// </summary>
        AllowReturn = 1,

        /// <summary>
        ///     Allows the usage of the Break Keyword
        /// </summary>
        AllowBreak = 2,

        /// <summary>
        ///     Allows the usage of the Continue Keyword
        /// </summary>
        AllowContinue = 4,

        /// <summary>
        ///     Allows the usage of the Throw Keyword
        /// </summary>
        AllowThrow = 8,


        /// <summary>
        ///     Indicates that the current scope should capture the return and should not pass it to the parent scope
        /// </summary>
        CaptureReturn = 16,

        /// <summary>
        ///     Indicates that the current scope should capture the break and should not pass it to the parent scope
        /// </summary>
        CaptureBreak = 32,

        /// <summary>
        ///     Indicates that the current scope should capture the continue and should not pass it to the parent scope
        /// </summary>
        CaptureContinue = 64,

        /// <summary>
        ///     Indicates that the current scope should capture the throw and should not pass it to the parent scope
        /// </summary>
        CaptureThrow = 128,

        /// <summary>
        ///     Shorthand for AllowReturn | CaptureReturn
        /// </summary>
        Returnable = AllowReturn | CaptureReturn,

        /// <summary>
        ///     Shorthand for AllowBreak | CaptureBreak
        /// </summary>
        Breakable = AllowBreak | CaptureBreak,

        /// <summary>
        ///     Shorthand for AllowContinue | CaptureContinue
        /// </summary>
        Continuable = AllowContinue | CaptureContinue,

        /// <summary>
        ///     Shorthand for AllowThrow
        /// </summary>
        Throwable = AllowThrow,

        /// <summary>
        ///     Shorthand for Root Scope
        /// </summary>
        RootScope = Throwable,
    }
}