using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CameraTorrent
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp =>
            {
                var baseAddress = builder.HostEnvironment.BaseAddress;
                return new HttpClient { BaseAddress = new Uri(baseAddress) };
            });

            await builder.Build().RunAsync();
        }
    }
}