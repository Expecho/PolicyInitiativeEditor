using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;
using PolicyInitiativeBuilder.Models;
using PolicyInitiativeBuilder.Services;

namespace PolicyInitiativeBuilder.Components.Layout;

public partial class MainLayout(TenantService tenantService, IMemoryCache tenantsCache)
{
    private IEnumerable<Tenant> tenants = [];

    [Parameter]
    public Tenant Tenant { get; set; } = Tenant.Empty;

    [CascadingParameter]
    private Task<AuthenticationState>? authenticationState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (authenticationState is not null)
        {
            var authState = await authenticationState;
            var user = authState?.User;

            if (user?.Identity is not null && user.Identity.IsAuthenticated)
            {
                tenants = (await tenantService.GetTenantsAsync().ToListAsync()).OrderBy(t => t.DisplayName);
                Tenant = tenants.First(tenant => tenant.Id == Guid.Parse(user.GetTenantId()!));
            }
        }
    }

    private void OnTenantChanged(Tenant tenant)
    {
        var challengeUri = $"/Login/?handler=SignIn&tenant={tenant.Id}";
        Navigation.NavigateTo(challengeUri, true);
    }
}
