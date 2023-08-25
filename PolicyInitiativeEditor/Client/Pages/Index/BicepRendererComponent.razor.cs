using Microsoft.AspNetCore.Components;
using PolicyInitiativeEditor.Client.Domain;
using PolicyInitiativeEditor.Shared;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class BicepRendererComponent
    {
        private string bicep = string.Empty;

        [Parameter]
        public IList<Policy>? Policies { get; set; }

        override protected void OnParametersSet()
        {
            bicep = BicepBuilder.CreateBicepFromPolicies(Policies!);
        }
    }
}
