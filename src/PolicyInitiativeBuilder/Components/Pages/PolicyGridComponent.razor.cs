using Microsoft.AspNetCore.Components;
using PolicyInitiativeBuilder.Models;

namespace PolicyInitiativeBuilder.Components.Pages;

public partial class PolicyGridComponent
{
    [Parameter]
    public IEnumerable<Policy>? Policies { get; set; }

    [Parameter]
    public IList<Policy>? SelectedPolicies { get; set; }

    [Parameter]
    public EventCallback<IList<Policy>> SelectedPoliciesChanged { get; set; }

    private void OnSelectedPoliciesChanged(IList<Policy> policies)
    {
        SelectedPoliciesChanged.InvokeAsync(policies);
    }
}
