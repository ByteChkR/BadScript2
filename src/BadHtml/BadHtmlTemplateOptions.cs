using BadScript2;

namespace BadHtml;

/// <summary>
///     Options for the Html Template Engine
/// </summary>
public class BadHtmlTemplateOptions
{
    public BadRuntime? Runtime = null;
    public bool SkipEmptyTextNodes = false;
}