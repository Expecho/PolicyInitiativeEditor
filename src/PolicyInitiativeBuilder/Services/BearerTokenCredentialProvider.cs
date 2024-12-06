using Azure.Core;
using Microsoft.Identity.Web;

namespace PolicyInitiativeBuilder.Services;

public class BearerTokenCredentialProvider(ITokenAcquisition tokenAcquisition, Guid tenantId) : TokenCredential
{
    public BearerTokenCredentialProvider(ITokenAcquisition tokenAcquisition) : this(tokenAcquisition, Guid.Empty)
    {

    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(GetTokenAsync(tenantId).GetAwaiter().GetResult(), DateTime.Now.AddDays(1));
    }

    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(await GetTokenAsync(tenantId), DateTime.Now.AddDays(1));

    }

    private async Task<string> GetTokenAsync(Guid tenantId)
    {
        var options = new TokenAcquisitionOptions();
        if (tenantId != Guid.Empty)
        {
            options.Tenant = tenantId.ToString();
        }

        var tenant = tenantId == Guid.Empty ? null : tenantId.ToString();


        var token = await tokenAcquisition.GetAccessTokenForUserAsync(["https://management.azure.com/user_impersonation"], tenantId: tenant, tokenAcquisitionOptions: options);
        return token;
    }
}
