using Azure.ResourceManager;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Azure;
using PolicyInitiativeEditor.Client;
using PolicyInitiativeEditor.Client.Domain;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<DialogService>();
builder.Services.AddSingleton<BicepBuilder>();
builder.Services.AddSingleton<AzureResourceRepository>();
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://management.core.windows.net/user_impersonation");
});
var scopes = new List<string>() {
    "https://management.core.windows.net/user_impersonation",
};

builder.Services.AddAzureClients(clientFactoryBuilder =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();

    var tokenProvider = serviceProvider.GetRequiredService<IAccessTokenProvider>();
    var navigation = serviceProvider.GetRequiredService<NavigationManager>();

    clientFactoryBuilder.AddClient<ArmClient, ArmClientOptions>(_ =>
    {
        return new ArmClient(new BearerTokenCredential(tokenProvider, navigation, scopes));
    });
});

await builder.Build().RunAsync();
