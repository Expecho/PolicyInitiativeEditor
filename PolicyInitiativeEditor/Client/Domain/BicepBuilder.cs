using PolicyInitiativeEditor.Shared;
using System.Text;

namespace PolicyInitiativeEditor.Client.Domain
{
    public class BicepBuilder
    {
        private readonly ILogger<BicepBuilder> logger;

        public BicepBuilder(ILogger<BicepBuilder> logger)
        {
            this.logger = logger;
        }

        public string CreateBicepFromPolicies(IEnumerable<Policy> policies, Options options)
        {
            var bicep = new StringBuilder();

            foreach (var policy in policies)
            {
                bicep.AppendLine("{");
                bicep.AppendLine($"\t// {policy.Name}");
                bicep.AppendLine($"\tpolicyDefinitionId: '{policy.Id}'");
                bicep.AppendLine("\tparameters: {");
                
                foreach (var parameter in policy.Parameters)
                {
                    bicep.AppendLine($"\t\t{parameter.Name}:{{");
                    bicep.AppendLine($"\t\t\tvalue: {parameter.DefaultValue} // {parameter.Type} {parameter.AllowedValues}");
                    bicep.AppendLine("\t\t}");
                }

                bicep.AppendLine("\t}");
                bicep.AppendLine("}");
            }

            return bicep.ToString();
        }
    }

    public class Options
    {
        public bool IncludePolicyDefinitionsSection { get; set; }
        public bool IncludeEffectParamater { get; set; }
    }
}
