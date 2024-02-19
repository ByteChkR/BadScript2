// See https://aka.ms/new-console-template for more information

using BadHtml;

using BadScript2;
using BadScript2.Debugger;
using BadScript2.Interop.Common;
using BadScript2.Interop.Html;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

internal static class Program
{
    private static void GenerateDocumentation(BadRuntime runtime)
    {
        //Load the template
        BadHtmlTemplate template = BadHtmlTemplate.Create("templates/docs.bhtml");

        //Run the template and write the result to a file
        File.WriteAllText("docs.html", template.Run(null, new BadHtmlTemplateOptions { Runtime = runtime }));
    }


    private static async Task GenerateFoodLibrary(BadRuntime runtime)
    {
        //Load the template
        BadHtmlTemplate template = BadHtmlTemplate.Create("templates/food.bhtml");

        string url = "https://www.fruityvice.com/api/fruit/all";
        using HttpClient client = new HttpClient();

        BadObject jsonData = BadJson.FromJson(await client.GetStringAsync(url));

        // All templates that depend on external data need a model object that the template can pull data from.
        // The model object can be any object that BadScript2 can convert to a BadObject.
        // The model object can also be a BadObject directly.
        BadTable model = new BadTable();

        //Set the Data
        model.SetProperty("Fruits", jsonData);

        //(Optional) Property that will be used to sort the fruit list.
        model.SetProperty("Order", "id"); //Order By ID


        //Run the template with the model and write the result to a file
        await File.WriteAllTextAsync("foodById.html", template.Run(model, new BadHtmlTemplateOptions { Runtime = runtime }));


        //(Optional) Property that will be used to sort the fruit list.
        model.SetProperty("Order", "name"); //Order By Name

        await File.WriteAllTextAsync("foodByName.html", template.Run(model, new BadHtmlTemplateOptions { Runtime = runtime }));
    }


    private static void Main()
    {
        //Initialize Engine(not part of this tutorial)
        using BadRuntime? runtime = new BadRuntime()
            .UseCommonInterop()
            .UseLinqApi()
            .UseFileSystemApi()
            .UseJsonApi()
            .UseCompilerApi()
            .UseHtmlApi();

        Console.WriteLine("Html Templates Example");


        // BadHtml is a wrapper around the HtmlAgilityPack library and BadScript2.
        // It adds custom syntax to the html files to allow for generating html files with BadScript2.
        // The syntax is very similar to the popular Svelte syntax.


        GenerateDocumentation(runtime);

        Task.WaitAll(GenerateFoodLibrary(runtime));
    }
}