﻿using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using PolicyInitiativeEditor.Client.Models;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class Index
    {
        private IEnumerable<Tenant> tenants = new List<Tenant>();
        private IEnumerable<Policy> policies = new List<Policy>();
        private IList<Policy> selectedPolicies = new List<Policy>();
        private string existingInitiative = string.Empty;
        private bool busyInitializing = true;

        protected override async Task OnInitializedAsync()
        {
            tenants = await azureResourceRepository.GetTenantsAsync().ToListAsync();
            busyInitializing = false;
        }

        private async Task OnSelectedTenantChanged(Tenant tenant)
        {
           policies = await azureResourceRepository.GetPoliciesAsync(tenant).ToListAsync();
        }

        
        private void OnSelectedPoliciesChanged(IList<Policy> policies)
        {
            selectedPolicies = policies;
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

        private static List<string> FindPolicies(List<string> bicep)
        {
            return bicep.FindAll(s => s.Contains("policyDefinitionId: '")).Select(s =>
            {
                return s[(s.IndexOf("'")+1)..s.LastIndexOf("'")];
            }).ToList();
        }
    }
}
