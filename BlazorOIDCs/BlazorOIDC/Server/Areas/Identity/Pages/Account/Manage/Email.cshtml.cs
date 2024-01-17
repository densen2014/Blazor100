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

namespace BlazorOIDC.Server.Areas.Identity.Pages.Account.Manage;

public class EmailModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public EmailModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    [Display(Name = "邮箱")]
    public string Email { get; set; }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>
    /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
    /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

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
        [Display(Name = "新邮箱")]
        public string NewEmail { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        var email = await _userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email,
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmailChange",
                pageHandler: null,
                values: new { area = "Identity", userId, email = Input.NewEmail, code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.NewEmail,
                "Confirm your email",
                $"请通过以下方式确认您的帐户 <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>点击这里</a>.");

            StatusMessage = "Confirmation link to change email sent. Please check your email.";
            return RedirectToPage();
        }

        StatusMessage = "Your email is unchanged.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId, code },
            protocol: Request.Scheme);
        await _emailSender.SendEmailAsync(
            email,
            "Confirm your email",
            $"请通过以下方式确认您的帐户 <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>点击这里</a>.");

        StatusMessage = "Verification email sent. Please check your email.";
        return RedirectToPage();
    }
}
