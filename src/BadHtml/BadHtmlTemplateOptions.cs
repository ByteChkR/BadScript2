using BadScript2.Runtime;

namespace BadHtml;

/// <summary>
///     Options for the Html Template Engine
/// </summary>
public class BadHtmlTemplateOptions
{
    public BadExecutionContextOptions? ContextOptions = null;
    public bool SkipEmptyTextNodes = false;
}