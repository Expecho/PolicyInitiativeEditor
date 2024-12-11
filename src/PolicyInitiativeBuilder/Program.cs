using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PolicyInitiativeBuilder.Components;
using PolicyInitiativeBuilder.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var initialScopes = new[] { "https://management.azure.com/user_impersonation" };

builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddScoped<PolicyService>();
builder.Services.AddScoped<BicepBuilder>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        .AddInMemoryTokenCaches();

builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();


WebApplication? app = null;

builder.Services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Prompt = "select_account";

    var redirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
    options.Events.OnRedirectToIdentityProvider = async context =>
    {
        if (app != null)
        {
            var email = context.HttpContext!.User.Identity!.Name;
            var tenantsCache = app.Services.GetRequiredService<IMemoryCache>();
            if (email != null && tenantsCache.TryGetValue<string>(email, out var tenantId))
            {
                var address = context.ProtocolMessage.IssuerAddress.Replace("common", tenantId!);
                context.ProtocolMessage.IssuerAddress = address;
            }
        }

        await redirectToIdentityProvider(context);
    };
});


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRadzenComponents();

app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseForwardedHeaders();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorPages();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
