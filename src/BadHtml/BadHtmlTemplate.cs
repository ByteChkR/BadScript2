using System.IO;

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
        m_Source = File.ReadAllText(m_FilePath);
    }

    public HtmlDocument RunTemplate(object? model = null)
    {
        string src = GetSource();
        HtmlDocument input = new HtmlDocument();
        input.LoadHtml(src);
        HtmlDocument output = new HtmlDocument();
        BadExecutionContext executionContext = BadExecutionContextOptions.Default.Build();

        BadObject mod = model as BadObject ?? BadObject.Wrap(model);
        executionContext.Scope.DefineVariable("Model", mod, executionContext.Scope, new BadPropertyInfo(null, true));

        foreach (HtmlNode node in input.DocumentNode.ChildNodes)
        {
            BadHtmlContext ctx = new BadHtmlContext(node, output.DocumentNode, executionContext, m_FilePath, src);
            BadHtmlNodeTransformer.Transform(ctx);
        }

        return output;
    }

    public string Run(object? model = null)
    {
        return RunTemplate(model).DocumentNode.OuterHtml;
    }

    public static BadHtmlTemplate Create(string file) => new BadHtmlTemplate(file);
}