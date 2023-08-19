using PolicyInitiativeEditor.Client.Domain;
using PolicyInitiativeEditor.Shared;
using System.Net.Http.Json;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class Index
    {
        private IEnumerable<Policy>? policies;
        private IList<Policy>? selectedPolicies;

        protected override async Task OnInitializedAsync()
        {
            policies = await Http.GetFromJsonAsync<IEnumerable<Policy>>("policy");
        }

        private void OnSelectedPoliciesChanged(IList<Policy> policies)
        {
            selectedPolicies = policies;
        }
    }
}
