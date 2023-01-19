// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using b16blazorIDS2.Models;

namespace b16blazorIDS2.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<WebAppIdentityUser> _userManager;
        private readonly SignInManager<WebAppIdentityUser> _signInManager;

        public IndexModel(
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
        [Display(Name = "用户名")]
        public string Username { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算使用
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算用于
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算用于
        /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// 此 API 支持 ASP.NET Core Identity 默认 UI 基础结构，不打算用于
            /// 直接来自您的代码。此 API 可能会在未来的版本中更改或删除。
            /// </summary>
            [Phone]
            [Display(Name = "电话号码")]
            public string PhoneNumber { get; set; }


            [Display(Name = "名称")]
            public string Name { get; set; }
        }

        private async Task LoadAsync(WebAppIdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var name = user.Name;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = name
            };
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

        public async Task<IActionResult> OnPostAsync()
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

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "尝试设置电话号码时出现意外错误.";
                    return RedirectToPage();
                }
            }

            var name = user.Name;
            if (Input.Name != name)
            {
                user.Name = Input.Name;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "你的个人资料已经更新";
            return RedirectToPage();
        }
    }
}
