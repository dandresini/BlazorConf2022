using BlazorConf2022.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BlazorConf2022.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorConf2022.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureB2C", options.ProviderOptions.Authentication);

    options.ProviderOptions.DefaultAccessTokenScopes.Add($"https://{builder.Configuration["AzureB2C:TenantName"]}/backendapi/api.write");
    options.ProviderOptions.AdditionalScopesToConsent.Add($"https://{builder.Configuration["AzureB2C:TenantName"]}/backendapi/api.read");
    options.ProviderOptions.LoginMode = "redirect";

});

await builder.Build().RunAsync();
