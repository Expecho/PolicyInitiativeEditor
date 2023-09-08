using Microsoft.AspNetCore.Components;
using PolicyInitiativeEditor.Client.Domain;
using PolicyInitiativeEditor.Shared;
using System.Reflection;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class BicepRendererComponent
    {
        [Parameter]
        public string? Bicep { get; set; }

        [Parameter]
        public IList<Policy>? Policies { get; set; }

        override protected void OnParametersSet()
        {
            if (string.IsNullOrWhiteSpace(Bicep))
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PolicyInitiativeEditor.Client.Resources.template.bicep");
                using var reader = new StreamReader(stream!);
                {
                    Bicep = reader.ReadToEnd();
                }
            }

            Bicep = BicepBuilder.CreateBicepFromPolicies(Bicep, Policies!);
        }
    }
}
