using System.Text.Json.Nodes;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PolicyInitiativeEditor.Client.Models;

namespace PolicyInitiativeEditor.Client.Domain
{
    public class AzureResourceRepository(ArmClient armClient, IConfiguration configuration)
    {
        private readonly ArmClient armClient = armClient;
        private readonly IConfiguration configuration = configuration;
        
        public async IAsyncEnumerable<Policy> GetPoliciesAsync()
        {
            var tenantCollection = armClient.GetTenants();
            List<TenantResource> tenants;

            try
            {
                tenants = await tenantCollection.ToListAsync();
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                yield break;
            }

            var tenantId = configuration.GetValue<string?>("AzureAd:Authority")!.Split("/").Last();

            var tenant = tenants.First(tenant => tenant.Data.TenantId == Guid.Parse(tenantId));
            var managementGroupId = configuration.GetValue<string?>("ManagementGroupId");

            if (string.IsNullOrWhiteSpace(managementGroupId))
            {
                managementGroupId = tenant.Data.TenantId.ToString();
            }

            var rootManagementGroup = await tenant.GetManagementGroupAsync(managementGroupId);
            var policyDefinitions = rootManagementGroup.Value.GetManagementGroupPolicyDefinitions();

            await foreach (var page in policyDefinitions.GetAllAsync().AsPages(pageSizeHint: 100))
            {
                foreach (var policy in page.Values)
                {
                    yield return new Policy(
                            policy.Data.Id!,
                            policy.Data.DisplayName,
                            policy.Data.PolicyType.ToString()!,
                            policy.Data.Metadata!.ToObjectFromJson<JsonNode>()["category"]!.ToString(),
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
}
