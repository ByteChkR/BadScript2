using BadScript2;

namespace BadHtml;

/// <summary>
///     Options for the Html Template Engine
/// </summary>
public class BadHtmlTemplateOptions
{
    /// <summary>
    /// The Runtime to use for the Template
    /// </summary>
    public BadRuntime? Runtime = null;
    
    /// <summary>
    /// If true, empty Html Text Nodes will be omitted from the output
    /// </summary>
    public bool SkipEmptyTextNodes = false;
}