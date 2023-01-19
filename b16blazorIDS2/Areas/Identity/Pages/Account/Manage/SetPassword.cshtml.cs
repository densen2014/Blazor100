// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using b16blazorIDS2.Models;

namespace b16blazorIDS2.Areas.Identity.Pages.Account.Manage
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<WebAppIdentityUser> _userManager;
        private readonly SignInManager<WebAppIdentityUser> _signInManager;

        public SetPasswordModel(
            UserManager<WebAppIdentityUser> userManager,
            SignInManager<WebAppIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        [TempData]
        public string StatusMessage { get; set; }

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
            [StringLength(100, ErrorMessage = "{0} 的长度必须至少为 {2}，最多为 {1} 个字符.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "新密码")]
            public string NewPassword { get; set; }

            /// <summary>
            /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
            /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "确认新密码")]
            [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToPage("./ChangePassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"无法加载具有 ID 的用户 '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been set.";

            return RedirectToPage();
        }
    }
}
