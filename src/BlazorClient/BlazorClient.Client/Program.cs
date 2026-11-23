using BlazorClient.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiUrl = builder.Configuration["ApiUrl"]
    ?? throw new InvalidOperationException("Falta ApiUrl en appsettings.");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

builder.Services.AddScoped<SesionUsuario>();
await builder.Build().RunAsync();