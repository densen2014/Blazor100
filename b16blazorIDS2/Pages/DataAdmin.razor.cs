// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b16blazorIDS2.Models;
using Blazor100.Service;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using static b16blazorIDS2.Pages.Index;

namespace b16blazorIDS2.Pages
{
    public partial class DataAdmin
    {
        [Inject]
        [NotNull]
        protected UserManager<WebAppIdentityUser>? UserManager { get; set; }

        [Inject]
        [NotNull]
        protected RoleManager<IdentityRole>? RoleManager { get; set; }

        [Inject]
        [NotNull]
        ToastService? ToastService { get; set; }


        public bool IsExcel { get; set; }
        public bool DoubleClickToEdit { get; set; } = true;
 
        private Task IsExcelToggle()
        {
            IsExcel = !IsExcel;
            DoubleClickToEdit = !IsExcel;
            StateHasChanged();
            return Task.CompletedTask;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender) return;

            var RoleResult = await RoleManager.FindByNameAsync(AuthorizeRoles.Admin.ToString());
            if (RoleResult == null)
            {
                await RoleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Admin.ToString()));
                await ToastService.Information("Admin Role Created");
            }

            var user = await UserManager.FindByNameAsync("test@app.com");
            if (user != null)
            {
                var UserResult = await UserManager.IsInRoleAsync(user, AuthorizeRoles.Admin.ToString());
                if (!UserResult)
                {
                    await UserManager.AddToRoleAsync(user, AuthorizeRoles.Admin.ToString());
                    await ToastService.Information("Admin Role Added to test@app.com");
                }
            }

            var chekRole = RoleManager.RoleExistsAsync(AuthorizeRoles.Admin.ToString());
            if (chekRole.Result == false)
            {
                await RoleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Admin.ToString()));
                await ToastService.Information("Admin Role Created");
            }

            chekRole = RoleManager.RoleExistsAsync(AuthorizeRoles.Superuser.ToString());
            if (chekRole.Result == false)
            {
                await RoleManager.CreateAsync(new IdentityRole(AuthorizeRoles.Superuser.ToString()));
                await ToastService.Information("Superuser Role Created");

            }


        }

    }
}
