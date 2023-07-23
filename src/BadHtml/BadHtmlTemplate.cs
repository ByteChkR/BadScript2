using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml;

/// <summary>
///     Implements a Html Template
/// </summary>
public class BadHtmlTemplate
{
	/// <summary>
	///     The Filepath of the Template
	/// </summary>
	private readonly string m_FilePath;

	/// <summary>
	///     The Source code of the Template(gets loaded on first template run)
	/// </summary>
	private string? m_Source;

	/// <summary>
	///     Constructs a new Template
	/// </summary>
	/// <param name="filePath">File path of the template</param>
	private BadHtmlTemplate(string filePath)
	{
		m_FilePath = filePath;
	}

	/// <summary>
	///     The Filepath of the Template
	/// </summary>
	public string FilePath => m_FilePath;

	/// <summary>
	///     Returns the source code of the template. Loads the source code if it is not loaded yet.
	/// </summary>
	/// <returns>Template Source</returns>
	private string GetSource()
	{
		if (m_Source == null)
		{
			Reload();
		}

		return m_Source!;
	}

	/// <summary>
	///     Reloads the Template Source
	/// </summary>
	public void Reload()
	{
		m_Source = BadFileSystem.ReadAllText(m_FilePath);
	}

	/// <summary>
	///     Runs the Template with the specified arguments
	/// </summary>
	/// <param name="model">The Model used within the template</param>
	/// <param name="options">The Template Options</param>
	/// <param name="caller">Optional Caller (if executed from within badscript, is used to get a full stacktrace on errors)</param>
	/// <returns>The Html Document that was generated</returns>
	public HtmlDocument RunTemplate(
		object? model = null,
		BadHtmlTemplateOptions? options = null,
		BadScope? caller = null)
	{
		options ??= new BadHtmlTemplateOptions();
		string src = GetSource();
		HtmlDocument input = new HtmlDocument();
		input.LoadHtml(src);
		HtmlDocument output = new HtmlDocument();
		BadExecutionContext executionContext = BadExecutionContextOptions.Default.Build();
		executionContext.Scope.SetCaller(caller);

		BadObject mod = model as BadObject ?? BadObject.Wrap(model);
		executionContext.Scope.DefineVariable("Model", mod, executionContext.Scope, new BadPropertyInfo(null, true));

		foreach (HtmlNode node in input.DocumentNode.ChildNodes)
		{
			BadHtmlContext ctx =
				new BadHtmlContext(node, output.DocumentNode, executionContext, m_FilePath, src, options);
			BadHtmlNodeTransformer.Transform(ctx);
		}

		return output;
	}

	/// <summary>
	///     Runs the Template with the specified arguments
	/// </summary>
	/// <param name="model">The Model used within the template</param>
	/// <param name="options">The Template Options</param>
	/// <param name="caller">Optional Caller (if executed from within badscript, is used to get a full stacktrace on errors)</param>
	/// <returns>The Html Source of the Generated Document</returns>
	public string Run(object? model = null, BadHtmlTemplateOptions? options = null, BadScope? caller = null)
	{
		return RunTemplate(model, options, caller).DocumentNode.OuterHtml;
	}

	/// <summary>
	///     Creates a new Template
	/// </summary>
	/// <param name="file">Template File Path</param>
	/// <returns>A Html Template Instance</returns>
	public static BadHtmlTemplate Create(string file)
	{
		return new BadHtmlTemplate(file);
	}
}
