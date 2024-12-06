using System.Reflection;
using Microsoft.AspNetCore.Components;
using PolicyInitiativeBuilder.Models;
using PolicyInitiativeBuilder.Services;

namespace PolicyInitiativeBuilder.Components.Pages;

public partial class BicepRendererComponent(BicepBuilder bicepBuilder)
{
    [Parameter]
    public string? Bicep { get; set; }

    [Parameter]
    public IList<Policy>? Policies { get; set; }

    override protected void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Bicep))
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PolicyInitiativeBuilder.Resources.template.bicep");
            using var reader = new StreamReader(stream!);
            {
                Bicep = reader.ReadToEnd();
            }
        }

        Bicep = bicepBuilder.CreateBicepFromPolicies(Bicep, Policies!);
    }
}
