using BadScript2.Runtime;

namespace BadHtml;

/// <summary>
///     Options for the Html Template Engine
/// </summary>
public class BadHtmlTemplateOptions
{
	public bool SkipEmptyTextNodes = false;
	public BadExecutionContextOptions? ContextOptions = null;
}
