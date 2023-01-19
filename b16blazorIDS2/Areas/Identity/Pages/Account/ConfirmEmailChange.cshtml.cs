// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using b16blazorIDS2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace b16blazorIDS2.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<WebAppIdentityUser> _userManager;
        private readonly SignInManager<WebAppIdentityUser> _signInManager;

        public ConfirmEmailChangeModel(UserManager<WebAppIdentityUser> userManager, SignInManager<WebAppIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"无法加载具有 ID 的用户 '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                StatusMessage = "更改电子邮件时出错.";
                return Page();
            }

            // 在我们的 UI 中，电子邮件和用户名是一样的，所以当我们更新电子邮件时
            // 我们需要更新用户名。
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = "更改用户名时出错.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "感谢您确认您的电子邮件更改.";
            return Page();
        }
    }
}
