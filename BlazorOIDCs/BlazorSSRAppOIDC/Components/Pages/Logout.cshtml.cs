using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PersonalToolKit.Server.Components.Pages;

public class LogoutModel : PageModel
{
    public async Task OnGet(string redirectUri)
    {
        await HttpContext.SignOutAsync("Cookies");
        await HttpContext.SignOutAsync("oidc", new AuthenticationProperties { RedirectUri = redirectUri });
    }

}
