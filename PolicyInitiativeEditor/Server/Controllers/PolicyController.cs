using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.AspNetCore.Mvc;
using PolicyInitiativeEditor.Shared;
using System.Text.Json.Nodes;

namespace PolicyInitiativeEditor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly ILogger<PolicyController> logger;

        public PolicyController(ILogger<PolicyController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async IAsyncEnumerable<Policy> Get()
        {
            var armClient = new ArmClient(new DefaultAzureCredential());
            var tenantCollection = armClient.GetTenants();
            var tenants = await tenantCollection.GetAllAsync().ToListAsync();
            var tenant = tenants.First();
            var policyDefinitions = (await tenant.GetManagementGroupAsync("testprod")).Value.GetManagementGroupPolicyDefinitions();

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