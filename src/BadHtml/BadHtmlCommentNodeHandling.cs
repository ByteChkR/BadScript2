namespace BadHtml;

/// <summary>
/// Defines how the Html Template Engine should handle Html Comment Nodes
/// </summary>
public enum BadHtmlCommentNodeHandling
{
    /// <summary>
    ///     Comments will be treated as normal nodes and will be included in the output(but not executed)
    /// </summary>
    Include,

    /// <summary>
    ///     Comments will be treated as normal nodes but will not be included in the output
    /// </summary>
    Skip,

    /// <summary>
    ///     Comments will be treated as code and will be executed by the template engine, their output will be included in the output
    /// </summary>
    Execute
}