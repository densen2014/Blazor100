// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using b16blazorIDS2.Models;

namespace b16blazorIDS2.Areas.Identity.Pages.Account.Manage
{
    public class EnableAuthenticatorModel : PageModel
    {
        private readonly UserManager<WebAppIdentityUser> _userManager;
        private readonly ILogger<EnableAuthenticatorModel> _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public EnableAuthenticatorModel(
            UserManager<WebAppIdentityUser> userManager,
            ILogger<EnableAuthenticatorModel> logger,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        public string SharedKey { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        public string AuthenticatorUri { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        [TempData]
        public string[] RecoveryCodes { get; set; }

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
            [StringLength(7, ErrorMessage = "{0} 的长度必须至少为 {2}，最多为 {1} 个字符.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "验证码")]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            // Strip spaces and hyphens
            var verificationCode = Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "验证码无效.");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await _userManager.GetUserIdAsync(user);
            _logger.LogInformation("ID 为'{UserId}'的用户已使用身份验证器应用启用 2FA.", userId);

            StatusMessage = "您的身份验证器应用已通过验证.";

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                RecoveryCodes = recoveryCodes.ToArray();
                return RedirectToPage("./ShowRecoveryCodes");
            }
            else
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(WebAppIdentityUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
