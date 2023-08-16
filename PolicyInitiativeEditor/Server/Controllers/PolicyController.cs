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
        private readonly ILogger<PolicyController> _logger;

        public PolicyController(ILogger<PolicyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async IAsyncEnumerable<PolicyDto> Get()
        {
            var armClient = new ArmClient(new DefaultAzureCredential());
            var tenantCollection = armClient.GetTenants();
            var tenants = await tenantCollection.GetAllAsync().ToListAsync();
            var tenant = tenants.First();
            var policyDefinitions = tenant.GetTenantPolicyDefinitions();
            
            await foreach (var page in policyDefinitions.GetAllAsync().AsPages())
            {
                foreach (var policy in page.Values)
                {
                    yield return new PolicyDto(
                            policy.Data.Id!,
                            policy.Data.DisplayName,
                            policy.Data.PolicyType.ToString(),
                            policy.Data.Metadata!.ToObjectFromJson<JsonNode>()["category"]!.ToString(),
                            policy.Data.Description);
                }
            }
        }
    }
}