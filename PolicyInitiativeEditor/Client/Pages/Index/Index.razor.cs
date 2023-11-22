using PolicyInitiativeEditor.Client.Models;
using Radzen;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class Index
    {
        private IEnumerable<Policy> policies = new List<Policy>();
        private IList<Policy> selectedPolicies = new List<Policy>();
        private string existingInitiative = string.Empty;
        private bool busyInitializing = true;

        protected override async Task OnInitializedAsync()
        {
            var settings = (Settings)await DialogService.OpenAsync<SettingsComponent>(string.Empty, options: new DialogOptions
            {
                ShowTitle = false
            });
            policies = await azureResourceRepository.GetPoliciesAsync(settings.Tenant).ToListAsync();

            if(settings.BicepTemplate != null)
            {
                SelectPoliciesBasedOnUploadedBicepTemplate(settings);
            }

            busyInitializing = false;
        }

        private void SelectPoliciesBasedOnUploadedBicepTemplate(dynamic settings)
        {
            existingInitiative = settings.BicepTemplate;
            var bicep = existingInitiative.Split("\n").ToList();

            var policiesInExisitingInitiative = FindPolicies(bicep);
            selectedPolicies = policies.Where(p => policiesInExisitingInitiative.Contains(p.Id)).ToList();
        }

        private void OnSelectedPoliciesChanged(IList<Policy> policies)
        {
            selectedPolicies = policies;
        }        

        private static List<string> FindPolicies(List<string> bicep)
        {
            return bicep.FindAll(s => s.Contains("policyDefinitionId: '")).Select(s =>
            {
                return s[(s.IndexOf("'") + 1)..s.LastIndexOf("'")];
            }).ToList();
        }
    }
}
