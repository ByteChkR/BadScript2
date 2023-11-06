using BadScript2;
using BadScript2.Runtime;

namespace BadHtml;

/// <summary>
///     Options for the Html Template Engine
/// </summary>
public class BadHtmlTemplateOptions
{
    public BadRuntime? Runtime = null;
    public bool SkipEmptyTextNodes = false;
}