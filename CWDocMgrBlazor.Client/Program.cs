using CWDocMgrBlazor.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register your delegating handler
builder.Services.AddTransient<HttpLoggingHandler>();

// Configure a named client; the platform provides the correct browser handler
builder.Services.AddHttpClient("Default", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddHttpMessageHandler<HttpLoggingHandler>();

// Expose the default HttpClient for @inject HttpClient
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Default"));

await builder.Build().RunAsync();