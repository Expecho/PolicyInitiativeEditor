using PolicyInitiativeEditor.Shared;
using System.Text;

namespace PolicyInitiativeEditor.Client.Domain
{
    public class BicepBuilder
    {
        private readonly StringBuilder initiative = new();
        private readonly StringBuilder includedPolicies = new();
        private readonly StringBuilder initiativeGroups = new();
        private readonly List<string> groupNames = new();

        public string CreateBicepFromPolicies(IEnumerable<Policy> policies)
        {
            initiative.Clear();
            includedPolicies.Clear();
            initiativeGroups.Clear();
            groupNames.Clear();


            foreach (var policy in policies)
            {
                OutputPolicy(policy);
                OutputGroup(policy);
            }

            initiative.AppendLine("policyDefinitionGroups: [");
            initiative.Append(initiativeGroups);
            initiative.AppendLine("]");

            initiative.AppendLine("policyDefinitions: [");
            initiative.Append(includedPolicies);
            initiative.AppendLine("]");

            return initiative.ToString();
        }

        private void OutputPolicy(Policy policy)
        {
            includedPolicies.AppendLine("\t{");
            includedPolicies.AppendLine($"\t\tpolicyDefinitionId: '{policy.Id}'");
            includedPolicies.AppendLine($"\t\tpolicyDefinitionReferenceId: '{policy.Name}'");
            includedPolicies.AppendLine("\t\tgroupNames: [");
            includedPolicies.AppendLine($"\t\t\t'{policy.Category}'");
            includedPolicies.AppendLine("\t\t]");

            OutputParametersIfAny(policy);

            includedPolicies.AppendLine("\t}");
        }

        private void OutputParametersIfAny(Policy policy)
        {
            if(!policy.Parameters.Any())
                return;

            includedPolicies.AppendLine("\t\tparameters: {");

            foreach (var parameter in policy.Parameters)
            {
                includedPolicies.AppendLine($"\t\t\t{parameter.Name}:{{");

                var parameterValue = parameter.DefaultValue?.Replace("\"", "'") ?? "[NOT_SET]";
                includedPolicies.AppendLine($"\t\t\t\tvalue: {parameterValue} // {parameter.Type} {parameter.AllowedValues}");
                includedPolicies.AppendLine("\t\t\t}");
            }

            includedPolicies.AppendLine("\t\t}");
        }

        private void OutputGroup(Policy policy)
        {
            if(groupNames.Contains(policy.Category))
                return;

            groupNames.Add(policy.Category);

            initiativeGroups.AppendLine("\t{");
            initiativeGroups.AppendLine($"\t\tname: '{policy.Category}'");
            initiativeGroups.AppendLine($"\t\tdisplayName: '{policy.Category}'");
            initiativeGroups.AppendLine($"\t\tcategory: '{policy.Category}'");
            initiativeGroups.AppendLine("\t}");
        }
    }
}
