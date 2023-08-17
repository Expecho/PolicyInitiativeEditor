using PolicyInitiativeEditor.Client.Domain;
using PolicyInitiativeEditor.Shared;
using System.Net.Http.Json;

namespace PolicyInitiativeEditor.Client.Pages
{
    public partial class Index
    {
        private IEnumerable<Policy>? policies;
        private IList<Policy>? selectedPolicies;
        private string bicep = string.Empty;
        private Options options = new();

        protected override async Task OnInitializedAsync()
        {
            policies = await Http.GetFromJsonAsync<IEnumerable<Policy>>("policy");
        }

        private void OnChangeOption(bool value)
        {
            bicep = BicepBuilder.CreateBicepFromPolicies(selectedPolicies!, options);
        }

        private void OnChangeTab(int tabIndex)
        {
            if (selectedPolicies != null && selectedPolicies.Count > 0 && tabIndex == 1)
                bicep = BicepBuilder.CreateBicepFromPolicies(selectedPolicies!, options);
        }
    }
}
