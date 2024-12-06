using System.Text.Json.Nodes;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Identity.Web;
using PolicyInitiativeBuilder.Models;

namespace PolicyInitiativeBuilder.Services;

public class PolicyService(ITokenAcquisition tokenAcquisition)
{
    public async IAsyncEnumerable<Policy> GetPoliciesForTenantAsync(Tenant tenant)
    {
        var armClient = new ArmClient(new BearerTokenCredentialProvider(tokenAcquisition, tenant.Id));
        var tenantCollection = armClient.GetTenants();
        var armTenant = tenantCollection.First(t => t.Data.TenantId == tenant.Id);
        ManagementGroupPolicyDefinitionCollection policyDefinitions;

        var rootManagementGroup = await armTenant.GetManagementGroupAsync(tenant.Id.ToString());
        policyDefinitions = rootManagementGroup.Value.GetManagementGroupPolicyDefinitions();

        await foreach (var page in policyDefinitions.GetAllAsync().AsPages(pageSizeHint: 100))
        {
            foreach (var policy in page.Values)
            {
                yield return new Policy(
                        policy.Data.Id!,
                        policy.Data.DisplayName,
                        policy.Data.PolicyType.ToString()!,
                        policy.Data.Metadata!.ToObjectFromJson<JsonNode>()["category"]?.ToString(),
                        policy.Data.Description,
                        policy.Data.Parameters.Select(p => new Parameter(
                            p.Key,
                            p.Value.ParameterType.ToString()!,
                            p.Value.DefaultValue?.ToString(),
                            p.Value.Metadata.DisplayName,
                            p.Value.AllowedValues == null || !p.Value.AllowedValues.Any() ? string.Empty : $"[{string.Join(",", p.Value.AllowedValues.Select(v => v.ToString()))}]"
                        )));
            }
        }
    }
}
