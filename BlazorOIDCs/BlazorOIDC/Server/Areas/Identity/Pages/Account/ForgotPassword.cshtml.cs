// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

#nullable disable

using BlazorOIDC.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace BlazorOIDC.Server.Areas.Identity.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // 不要透露用户不存在或未确认
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            // 有关如何启用帐户确认和密码重置的更多信息，请
            // 访问 https://go.microsoft.com/fwlink/?Link ID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "重设密码",
                $"请通过以下方式重置您的密码 <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>点击这里</a>.");

            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        return Page();
    }
}
