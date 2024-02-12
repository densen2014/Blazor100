using System.Security.Claims;

namespace BlazorSSRAppOIDC;

public static partial class RoleExtensions
{
    public static bool IsInRoleAny(this ClaimsPrincipal? user, string role)
    {
        if (user == null)
        {
            return false;
        }
        var roles = role.Split(',');
        foreach (var r in roles)
        {
            if (user.IsInRole(r))
            {
                return true;
            }
        }

        return false;
    }
}
