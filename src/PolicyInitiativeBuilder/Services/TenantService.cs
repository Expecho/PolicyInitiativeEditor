using Azure.ResourceManager;
using Microsoft.Identity.Web;
using PolicyInitiativeBuilder.Models;

namespace PolicyInitiativeBuilder.Services;

public class TenantService(ITokenAcquisition tokenAcquisition)
{
    public async IAsyncEnumerable<Tenant> GetTenantsAsync()
    {
        var armClient = new ArmClient(new BearerTokenCredentialProvider(tokenAcquisition));
        var tenantCollection = armClient.GetTenants();

        await foreach (var tenant in tenantCollection)
        {
            yield return new Tenant(
                tenant.Data.TenantId.GetValueOrDefault(),
                tenant.Data.DisplayName,
                tenant.Data.DefaultDomain);
        }
    }
}
