using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace PolicyInitiativeBuilder.Pages;

public class LoginModel(IMemoryCache tenantsCache) : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnGetSignIn([FromQuery] string tenant)
    {
        var email = User.Identity!.Name;
        if (email != null)
        {
            tenantsCache.Set(email!, tenant, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });
        }

        return Challenge(new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme);
    }
}
