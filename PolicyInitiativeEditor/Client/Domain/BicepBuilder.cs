using PolicyInitiativeEditor.Client.Models;

namespace PolicyInitiativeEditor.Client.Domain
{
    public class BicepBuilder
    {
        private List<string> initiative = new();
        private List<string> policiesInInitiative = new();
        private int indexOfPolicyDefinitionSection;

        public string CreateBicepFromPolicies(string sourceTemplate, IEnumerable<Policy> policies)
        {
            if(!initiative.Any())
                initiative = sourceTemplate.Split("\n").ToList();

            indexOfPolicyDefinitionSection = DetermineIndexOfPolicyDefinitionSection();
            policiesInInitiative = FindPolicies();

            foreach (var policy in policies.Where(policy => !policiesInInitiative.Contains(policy.Id)))
            {
                initiative.InsertRange(indexOfPolicyDefinitionSection, BuildPolicyOutput(policy));
            }

            foreach (var policy in policiesInInitiative.Where(policy => !policies.Select(p => p.Id).Contains(policy)))
            {
                RemovePolicy(policy);
            }

            return string.Join("\n", initiative);
        }

        private int DetermineIndexOfPolicyDefinitionSection()
        {
            var startIndex = initiative.FindIndex(0, s => s.Contains("policyDefinitions: ["));
            return startIndex + 1;
        }

        private static List<string> BuildPolicyOutput(Policy policy)
        {
            var policyId = policy.Type switch
            {
                "BuiltIn" => $"tenantResourceId('Microsoft.Authorization/policyDefinitions', '{policy.Id}')",
                _ => $"extensionResourceId(mgScope, 'Microsoft.Authorization/policyDefinitions', '{policy.Id}')"
            };

            var policyDefinition = new List<string>
            { 
                "\t{",
                $"\t\tpolicyDefinitionId: {policyId}",
                $"\t\tpolicyDefinitionReferenceId: '{policy.Name}'",
                "\t}"
            };

            policyDefinition.InsertRange(3, BuildPolicyParameterOutput(policy));

            return policyDefinition;
        }

        private static List<string> BuildPolicyParameterOutput(Policy policy)
        {
            var parameters = new List<string>();

            if (!policy.Parameters.Any())
                return parameters;

            parameters.Add("\t\tparameters: {");

            foreach (var parameter in policy.Parameters)
            {
                parameters.Add($"\t\t\t{parameter.Name}:{{");

                var parameterValue = parameter.DefaultValue?.Replace("\"", "'") ?? "[NOT_SET]";
                parameters.Add($"\t\t\t\tvalue: {parameterValue} // {parameter.Type} {parameter.AllowedValues}");
                parameters.Add("\t\t\t}");
            }

            parameters.Add("\t\t}");

            return parameters;
        }

        private void RemovePolicy(string policyIdentifier)
        {
            var startIndex = initiative.FindIndex(indexOfPolicyDefinitionSection, s => s.Contains(policyIdentifier));
            var countOfOpen = 1;
            var countOfClosed = 0;
            var index = startIndex;

            if(startIndex == -1)
                return;

            while (countOfOpen != countOfClosed && index < initiative.Count)
            {
                var line = initiative[index];
                if (line.Contains('{'))
                    ++countOfOpen;
                else if (line.Contains('}'))
                    ++countOfClosed;

                ++index;
            }

            if(index != initiative.Count)
                initiative.RemoveRange(startIndex - 1, index - startIndex + 1);
        }

        private List<string> FindPolicies()
        {
            return initiative.FindAll(s => s.Contains("policyDefinitionId: '")).Select(s =>
            {
                return s[(s.IndexOf("'") + 1)..s.LastIndexOf("'")];
            }).ToList();
        }
    }
}
