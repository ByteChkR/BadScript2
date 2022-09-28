namespace BadScript2.Parser.Expressions.Constant
{
    /// <summary>
    ///     Gets implemented by all Constant/Native Expressions to provide fast access to the value without unwrapping it
    ///     first.
    /// </summary>
    public interface IBadNativeExpression
    {
        /// <summary>
        ///     The Constant Value of this Expression
        /// </summary>
        object Value { get; }
    }
}