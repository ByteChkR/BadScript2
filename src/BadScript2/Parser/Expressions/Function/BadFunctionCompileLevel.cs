namespace BadScript2.Parser.Expressions.Function;

/// <summary>
/// The BadFunctionCompileLevel enum defines the different levels of compilation for a function.
/// </summary>
public enum BadFunctionCompileLevel
{
    /// <summary>
    /// None, the function is not compiled.
    /// </summary>
    None,
    /// <summary>
    /// The Function is compiled to Instructions
    /// </summary>
    Compiled,
    /// <summary>
    /// The function is compiled to Instructions with operator overrides disabled.
    /// </summary>
    CompiledFast,
}