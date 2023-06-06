// See https://aka.ms/new-console-template for more information

using BadHtml;

using BadScript2.Interop.Json;
using BadScript2.Runtime.Objects;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("Html Templates Example");


        // BadHtml is a wrapper around the HtmlAgilityPack library and BadScript2.
        // It adds custom syntax to the html files to allow for generating html files with BadScript2.
        // The syntax is very similar to the popular Svelte syntax.

        // The templates can now be used to generate html files.

        // All templates that depend on external data need a model object that the template can pull data from.
        // The model object can be any object that BadScript2 can convert to a BadObject.
        // The model object can also be a BadObject directly.
        BadObject model = BadObject.Null;//BadJson.FromJson(File.ReadAllText("templates/docs.json"));

        //Load the template
        BadHtmlTemplate template = new BadHtmlTemplate("templates/docs.bhtml");

        //Run the template with the model and write the result to a file
        File.WriteAllText("template.html", template.Run(model));
    }
}