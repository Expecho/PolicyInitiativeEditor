using Azure.ResourceManager;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Azure;
using PolicyInitiativeEditor.Client;
using PolicyInitiativeEditor.Client.Domain;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<BicepBuilder>();
builder.Services.AddSingleton<AzureResourceRepository>();
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://management.core.windows.net/user_impersonation");
    options.ProviderOptions.AdditionalScopesToConsent.Add("https://management.core.windows.net/user_impersonation");
});
var scopes = new List<string>() {
    "https://management.core.windows.net/user_impersonation",
    "https://management.core.windows.net"
};

builder.Services.AddAzureClients(b =>
{
    var sp = builder.Services.BuildServiceProvider();

    var tokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
    var navigation = sp.GetRequiredService<NavigationManager>();

    b.AddClient<ArmClient, ArmClientOptions>(o =>
    {
        return new ArmClient(new BearerTokenCredential(tokenProvider, navigation, scopes));
    });
});

await builder.Build().RunAsync();
