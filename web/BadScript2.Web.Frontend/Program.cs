using System.Runtime.CompilerServices;

using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BadScript2.Web.Frontend;

public class Program
{
    public static async Task Main(string[] args)
    {
        RuntimeHelpers.RunClassConstructor(typeof(BadObject).TypeHandle);
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(
            sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            }
        );

        await builder.Build().RunAsync();
    }
}