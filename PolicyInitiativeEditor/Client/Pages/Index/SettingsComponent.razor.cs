using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PolicyInitiativeEditor.Client.Models;

namespace PolicyInitiativeEditor.Client.Pages.Index
{
    public partial class SettingsComponent
    {
        private IEnumerable<Tenant> tenants = new List<Tenant>();

        public Tenant? SelectedTenant { get; set; }

        public string? BicepTemplate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            tenants = await azureResourceRepository.GetTenantsAsync().ToListAsync();
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            using var stream = e.File.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            stream.Close();

            BicepTemplate = Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
