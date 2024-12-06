using System.Text;
using Azure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PolicyInitiativeBuilder.Models;
using PolicyInitiativeBuilder.Services;
using Radzen;

namespace PolicyInitiativeBuilder.Components.Pages;

public partial class Home(PolicyService policyService, NotificationService notificationService)
{
    private IEnumerable<Policy> policies = [];
    private IList<Policy> selectedPolicies = [];
    private string existingInitiative = string.Empty;
    private bool busyInitializing = true;

    [CascadingParameter]
    protected Tenant Tenant { get; set; } = Tenant.Empty;

    protected override async Task OnInitializedAsync()
    {

        busyInitializing = false;
    }

    private async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        using var stream = e.File.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        stream.Close();

        existingInitiative = Encoding.UTF8.GetString(ms.ToArray());
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

    protected override async Task OnParametersSetAsync()
    {
        policies = [];
        if (Tenant.IsEmpty)
        {
            return;
        }

        try
        {
            if (!Tenant.IsEmpty)
            {
                policies = await policyService.GetPoliciesForTenantAsync(Tenant).ToListAsync();
            }
        }
        catch (RequestFailedException exception) when (exception.Status == 403)
        {
            notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Permission denied", CloseOnClick = true, Detail = "User needs permission to read the management groups of the tenant.", Duration = 5000 });
        }
        catch
        {
            notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "An error occured", CloseOnClick = true, Duration = 5000 });
        }

        await base.OnParametersSetAsync();
    }
}
