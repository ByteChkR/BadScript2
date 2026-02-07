using BadScript2;

namespace BadHtml;

/// <summary>
///     Options for the Html Template Engine
/// </summary>
public class BadHtmlTemplateOptions
{
    /// <summary>
    ///     The Runtime to use for the Template
    /// </summary>
    public BadRuntime? Runtime = null;

    /// <summary>
    ///     If true, empty Html Text Nodes will be omitted from the output
    /// </summary>
    public bool SkipEmptyTextNodes = false;
    
    /// <summary>
    /// Sets the handling of Html Comment Nodes, if set to Skip, comment nodes will be treated as normal nodes but will not be included in the output, if set to Execute, comment nodes will be treated as code and will be executed by the template engine, their output will be included in the output·
    /// </summary>
    public BadHtmlCommentNodeHandling CommentNodeHandling = BadHtmlCommentNodeHandling.Include;
    
}