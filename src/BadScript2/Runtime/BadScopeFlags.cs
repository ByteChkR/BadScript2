namespace BadScript2.Runtime
{
    [Flags]
    public enum BadScopeFlags
    {
        None = 0,

        AllowReturn = 1,
        AllowBreak = 2,
        AllowContinue = 4,
        AllowThrow = 8,
        CaptureReturn = 16,
        CaptureBreak = 32,
        CaptureContinue = 64,
        CaptureThrow = 128,

        Returnable = AllowReturn | CaptureReturn,
        Breakable = AllowBreak | CaptureBreak,
        Continuable = AllowContinue | CaptureContinue,
        Throwable = AllowThrow,

        RootScope = Throwable,
    }
}