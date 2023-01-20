// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b16blazorIDS2.Enum;
using b16blazorIDS2.Models;
using Blazor100.Service;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using static b16blazorIDS2.Pages.Index;

namespace b16blazorIDS2.Pages
{
    public partial class DataAdmin
    {
        [CascadingParameter]
        [NotNull]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        [NotNull]
        private UserManager<WebAppIdentityUser>? UserManager { get; set; }

        [Inject]
        [NotNull]
        private RoleManager<IdentityRole>? RoleManager { get; set; }

        [Inject]
        [NotNull]
        ToastService? ToastService { get; set; }

        WebAppIdentityUser objUser = new WebAppIdentityUser();
        string CurrentUserRole { get; set; } = AuthorizeRoles.User.ToString();
        string strError = "";


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

            await AddNewRole(AuthorizeRoles.Admin);
            await AddNewRole(AuthorizeRoles.Superuser);
            await AddNewRole(AuthorizeRoles.User);
            await AddNewRole(AuthorizeRoles.R110);
            await AddNewRole(AuthorizeRoles.R120);
            await AddNewRole(AuthorizeRoles.R130);
            await AddNewRole(AuthorizeRoles.R140);

            await AddUser("admin", AuthorizeRoles.Admin);
            await AddUser("super", AuthorizeRoles.Superuser);
            await AddUser("u01", AuthorizeRoles.User);
            await AddUser("u02", AuthorizeRoles.User);
            await AddUser("u03", AuthorizeRoles.User);
            await AddUser("u04", AuthorizeRoles.User);
            await AddUser("r01", AuthorizeRoles.R110);
            await AddUser("r02", AuthorizeRoles.R120);

            var user = (await AuthenticationStateTask).User;
            if (user?.Identity?.Name != null)
            {
                var userexist = await UserManager.FindByNameAsync(user.Identity.Name);
                if (userexist != null)
                {
                    var UserResult = await UserManager.IsInRoleAsync(userexist, AuthorizeRoles.Admin.ToString());
                    if (!UserResult)
                    {
                        userexist.UserRole = AuthorizeRoles.Admin.ToString();  
                        await UserManager.UpdateAsync(userexist);
                        
                        await UserManager.AddToRoleAsync(userexist, AuthorizeRoles.Admin.ToString());
                        await ToastService.Information($"Admin Role Added to {user.Identity.Name}");
                    }
                }
            }

        }

        private async Task<Task<bool>> AddNewRole(AuthorizeRoles AuthorizeRole)
        {
            var chekRole = RoleManager.RoleExistsAsync(AuthorizeRole.ToString());
            if (chekRole.Result == false)
            {
                await RoleManager.CreateAsync(new IdentityRole(AuthorizeRole.ToString()));
                await ToastService.Information($"{AuthorizeRole} Role Created");

            }

            return chekRole;
        }

        async Task AddUser(string UserName, AuthorizeRoles UserRole, WebAppIdentityUser? newUser = null) => await AddUser(UserName, UserRole.ToString(), newUser);
        async Task AddUser(string UserName, string UserRole, WebAppIdentityUser? newUser = null)
        {
            var user = await UserManager.FindByNameAsync(UserName);
            if (user != null) return;

            var NewUser = newUser ??
                new WebAppIdentityUser
                {
                    UserName = UserName,
                    Email = $"{UserName}@app.com",
                    PhoneNumber = UserName,
                    UUID = Guid.NewGuid().ToString(),
                    EmailConfirmed = true,
                    UserRole = UserRole,
                };
            await ToastService.Information($"{UserName}@app.com Created");

            var CreateResult =
                await UserManager
                .CreateAsync(NewUser, "0");

            if (!CreateResult.Succeeded)
            {
                if (CreateResult
                    .Errors
                    .FirstOrDefault() != null)
                {
                    strError =
                        CreateResult
                        .Errors
                        .FirstOrDefault()?
                        .Description ?? "";
                }
                else
                {
                    strError = "Create error";
                }
                await ToastService.Error(strError);

            }
            else
            {
                await UserManager.AddToRoleAsync(NewUser, CurrentUserRole);
                await ToastService.Information($"{UserName}@app.com Add To Role {CurrentUserRole}");
            }
        }

        async Task SaveUser(bool RefreshUsers)
        {
            try
            {
                #region 已存在的用户
                if (objUser.Id != "")
                {
                    // 获取用户
                    var user = await UserManager.FindByIdAsync(objUser.Id);
                    if (user == null) return;

                    // 更新电子邮件
                    user.UserName = objUser.UserName;
                    user.Email = objUser.Email;
                    user.UUID = objUser.UUID;
                    user.PhoneNumber = objUser.PhoneNumber;
                    user.EmailConfirmed = objUser.EmailConfirmed;
                    user.UserRole = CurrentUserRole;

                    // 更新用户
                    await UserManager.UpdateAsync(user);

                    // 只有当前值才更新密码
                    // 不是默认值
                    if (objUser.PasswordHash != "*****")
                    {
                        var resetToken =
                            await UserManager.GeneratePasswordResetTokenAsync(user);

                        var passworduser =
                            await UserManager.ResetPasswordAsync(
                                user,
                                resetToken,
                                objUser.PasswordHash ?? "");

                        if (!passworduser.Succeeded)
                        {
                            if (passworduser.Errors.FirstOrDefault() != null)
                            {
                                strError =
                                    passworduser
                                    .Errors
                                    .FirstOrDefault()?
                                    .Description ?? "";
                            }
                            else
                            {
                                strError = "Pasword error";
                            }

                            // 保持弹出窗口打开
                            return;
                        }
                    }

                    // 处理角色


                    var UserResult = await UserManager.GetRolesAsync(user);

                    if (UserResult.FirstOrDefault() != null)
                    {
                        //删掉组
                        foreach (var item in UserResult)
                        {
                            await UserManager.RemoveFromRoleAsync(user, item);

                        }
                    }
                    await UserManager.AddToRoleAsync(user, CurrentUserRole);


                    #endregion

                }
                else
                {
                    #region 新用户Insert new user

                    var NewUser =
                    new WebAppIdentityUser
                    {
                        UserName = objUser.UserName,
                        Email = objUser.Email,
                        PhoneNumber = objUser.PhoneNumber,
                        UUID = objUser.UUID,
                        EmailConfirmed = objUser.EmailConfirmed,
                        UserRole = CurrentUserRole,
                    };

                    var CreateResult =
                        await UserManager
                        .CreateAsync(NewUser, objUser.PasswordHash ?? "");

                    if (!CreateResult.Succeeded)
                    {
                        if (CreateResult
                            .Errors
                            .FirstOrDefault() != null)
                        {
                            strError =
                                CreateResult
                                .Errors
                                .FirstOrDefault()?
                                .Description ?? "";
                        }
                        else
                        {
                            strError = "Create error";
                        }

                        await ToastService.Error($"{objUser.Email} Save error {strError}");
                        // 保持弹出窗口打开
                        return;
                    }
                    else
                    {
                        await UserManager.AddToRoleAsync(NewUser, CurrentUserRole);
                    }
                    #endregion
                }


                if (RefreshUsers)
                {
                    // 刷新用户
                    //GetUsers();
                    //GetUserRolesAsync();
                    await ToastService.Information($"{objUser.Email} Save Role {CurrentUserRole}");
                }
            }
            catch (Exception ex)
            {
                strError = ex.GetBaseException().Message;
            }
        }

        async Task EditUser(WebAppIdentityUser _IdentityUser)
        {
            // 设置选中的用户
            // 作为当前用户
            objUser = _IdentityUser;

            // 获取用户
            var user = await UserManager.FindByIdAsync(objUser.Id);
            if (user != null)
            {
                // 用户是管理员角色吗？
                var UserResult = await UserManager.GetRolesAsync(user);

                if (UserResult.FirstOrDefault() == null)
                {
                    CurrentUserRole = "用户";
                }
                else
                {
                    CurrentUserRole = UserResult.First();
                }
            }

        }

    }
}
