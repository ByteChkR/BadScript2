using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml;

public class BadHtmlTemplate
{
	private readonly string m_FilePath;
	private string? m_Source;

	private BadHtmlTemplate(string filePath)
	{
		m_FilePath = filePath;
	}

	private string GetSource()
	{
		if (m_Source == null)
		{
			Reload();
		}

		return m_Source!;
	}

	public void Reload()
	{
		m_Source = BadFileSystem.ReadAllText(m_FilePath);
	}

	public HtmlDocument RunTemplate(object? model = null, BadHtmlTemplateOptions? options = null)
	{
		options ??= new BadHtmlTemplateOptions();
		string src = GetSource();
		HtmlDocument input = new HtmlDocument();
		input.LoadHtml(src);
		HtmlDocument output = new HtmlDocument();
		BadExecutionContext executionContext = BadExecutionContextOptions.Default.Build();

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

	public string Run(object? model = null, BadHtmlTemplateOptions? options = null)
	{
		return RunTemplate(model, options).DocumentNode.OuterHtml;
	}

	public static BadHtmlTemplate Create(string file)
	{
		return new BadHtmlTemplate(file);
	}
}
