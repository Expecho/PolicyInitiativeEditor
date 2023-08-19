using Microsoft.AspNetCore.Components;
using PolicyInitiativeEditor.Client.Domain;
using PolicyInitiativeEditor.Shared;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class BicepRendererComponent
    {
        private Options options = new();
        private string bicep = string.Empty;

        [Parameter]
        public IList<Policy>? Policies { get; set; }

        private void OnChangeOption()
        {
            bicep = BicepBuilder.CreateBicepFromPolicies(Policies!, options);
        }

        override protected void OnParametersSet()
        {
            bicep = BicepBuilder.CreateBicepFromPolicies(Policies!, options);
        }
    }
}
