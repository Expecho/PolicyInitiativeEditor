using System.Text.Json.Nodes;
using Azure.ResourceManager;
using PolicyInitiativeEditor.Client.Models;

namespace PolicyInitiativeEditor.Client.Domain
{
    public class AzureResourceRepository
    {
        private readonly ArmClient armClient;

        public AzureResourceRepository(ArmClient armClient)
        {
            this.armClient = armClient;
        }

        public async IAsyncEnumerable<Tenant> GetTenantsAsync()
        {
            var tenantCollection = armClient.GetTenants();
            await foreach (var tenant in tenantCollection.GetAllAsync())
            {
                yield return new Tenant(tenant.Data.TenantId.ToString()!, tenant.Data.DisplayName, tenant.Data.DefaultDomain);
            }
        }

        public async IAsyncEnumerable<Policy> GetPoliciesAsync(Tenant selectedTenant)
        {
            var tenantCollection = armClient.GetTenants();
            var tenant = await tenantCollection.FirstAsync(t => t.Data.TenantId == Guid.Parse(selectedTenant.Id));
            var customPoliciesManagementGroupId = "";//configuration.GetValue<string>("CustomPoliciesManagementGroupId");
            if (string.IsNullOrWhiteSpace(customPoliciesManagementGroupId))
            {
                customPoliciesManagementGroupId = tenant.Data.TenantId.ToString();
            }
            var policyDefinitions = (await tenant.GetManagementGroupAsync(customPoliciesManagementGroupId)).Value.GetManagementGroupPolicyDefinitions();

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
