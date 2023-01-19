// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using b16blazorIDS2.Models;

namespace b16blazorIDS2.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<WebAppIdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<WebAppIdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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

            /// <summary>
            /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
            /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "密码")]
            public string Password { get; set; }

            /// <summary>
            /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
            /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
            /// </summary>
            [Display(Name = "记住账号?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("用户登录.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("用户帐户被锁定.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "登录尝试无效.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
