// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

#nullable disable

using BlazorOIDC.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BlazorOIDC.Server.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterConfirmationModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _sender;

    public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender)
    {
        _userManager = userManager;
        _sender = sender;
    }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    public bool DisplayConfirmAccountLink { get; set; }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    public string EmailConfirmationUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
    {
        if (email == null)
        {
            return RedirectToPage("/Index");
        }
        returnUrl = returnUrl ?? Url.Content("~/");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"无法使用电子邮件登录用户 '{email}'.");
        }

        Email = email;
        // 添加真实的电子邮件发件人后，您应该删除此代码以确认帐户
        DisplayConfirmAccountLink = true;
        if (DisplayConfirmAccountLink)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            EmailConfirmationUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl },
                protocol: Request.Scheme);
        }

        return Page();
    }
}
