// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using BlazorOIDC.Server;
using BlazorOIDC.Server.Models;
using Densen.Models.ids;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Densen.Identity;

namespace BlazorOIDC.Server.Pages;

public partial class DataAdmin
{
    [CascadingParameter]
    [NotNull]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    [Inject]
    [NotNull]
    private UserManager<ApplicationUser>? UserManager { get; set; }

    [Inject]
    [NotNull]
    private RoleManager<IdentityRole>? RoleManager { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ToastService { get; set; }

    [Inject]
    [NotNull]
    private IFreeSql? fsql { get; set; }

    private ApplicationUser objUser = new ApplicationUser();

    private string CurrentUserRole { get; set; } = AuthorizeRoles.User.ToString();

    private string strError = "";
    private Expression<Func<AspNetUserRoles, bool>>? where = null;

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

        if (!firstRender)
        {
            return;
        }

        //IncludeMany 有第二个参数，可以进行二次查询前的修饰工作

        //列出用户带角色名称
        //var users = fsql.Select<AspNetUsers>().Where(a => a.Email == "test@test.com").IncludeMany(a=>a.AspNetUserRoless,then =>then.Include(c=>c.AspNetRoless)).ToOne();
        //var users2 = fsql.Select<AspNetUsers>().Where(a => a.Email == "test@test.com").IncludeMany(a => a.AspNetUserRoless, then => then.IncludeByPropertyName(nameof(AspNetUserRoles.AspNetRoless))).ToOne();
        //var users3 = fsql.Select<AspNetUsers>().Where(a => a.Email == "test@test.com").IncludeByPropertyName(nameof(AspNetUsers.AspNetUserRoless),then => then.IncludeByPropertyName(nameof(AspNetUserRoles.AspNetRoless))).ToOne();

        //列出Admin的用户
        //var users3 = fsql.Select<AspNetUsers>().IncludeMany(a => a.AspNetUserRoless, then => then.Include(c => c.AspNetRoless).Where (d=>d.AspNetRoless.Name== "Admin")).ToList();
        //users3.ForEach(a => System.Console.WriteLine(a.Email));

        //var users3 = fsql.Select<AspNetUsers>().IncludeMany(a => a.AspNetUserRoless, then => then.Include(c => c.AspNetRoless)).Where (d=>d.AspNetUserRoless.First().RoleName == "Admin").ToSql ();

        //where = where.And(d => d.AspNetRoless.Name == "Admin");
        //var test2 = where.ToString();

        //var dywhere = new DynamicFilterInfo { Field = "AspNetRoless.Name", Operator = DynamicFilterOperator.Equal, Value = "Admin" };

        //var users4 = fsql.Select<AspNetUsers>().IncludeByPropertyName("AspNetUserRoless", then => then.WhereDynamicFilter(dywhere)).ToList();

        //users4.ForEach(a => System.Console.WriteLine(a.Email));

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
                var UserResult = await UserManager.IsInRoleAsync(userexist, AuthorizeRoles.User.ToString());
                if (!UserResult)
                {
                    userexist.UserRole = AuthorizeRoles.User.ToString();
                    await UserManager.UpdateAsync(userexist);

                    await UserManager.AddToRoleAsync(userexist, AuthorizeRoles.User.ToString());
                    await ToastService.Information($"User Role Added to {user.Identity.Name}");
                }
                if (user?.Identity?.Name == "test@test.com")
                {
                    UserResult = await UserManager.IsInRoleAsync(userexist, AuthorizeRoles.Admin.ToString());
                    if (!UserResult)
                    {
                        userexist.UserRole = AuthorizeRoles.Admin.ToString();
                        await UserManager.UpdateAsync(userexist);

                        await UserManager.AddToRoleAsync(userexist, AuthorizeRoles.Admin.ToString());
                        await ToastService.Information($"Admin Role Added to {user.Identity.Name}");
                    }

                    UserResult = await UserManager.IsInRoleAsync(userexist, AuthorizeRoles.Superuser.ToString());
                    if (!UserResult)
                    {
                        userexist.UserRole = AuthorizeRoles.Superuser.ToString();
                        await UserManager.UpdateAsync(userexist);

                        await UserManager.AddToRoleAsync(userexist, AuthorizeRoles.Superuser.ToString());
                        await ToastService.Information($"Superuser Role Added to {user.Identity.Name}");
                    }
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

    private async Task AddUser(string UserName, AuthorizeRoles UserRole, ApplicationUser? newUser = null) => await AddUser(UserName, UserRole.ToString(), newUser);

    private async Task AddUser(string UserName, string UserRole, ApplicationUser? newUser = null)
    {
        var email = $"{UserName}@app.com";
        var user = await UserManager.FindByNameAsync(email);
        if (user != null)
        {
            return;
        }

        var NewUser = newUser ??
            new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = UserName,
                UUID = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                UserRole = UserRole,
            };
        await ToastService.Information($"{email} Created");

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
            await ToastService.Information($"{email} Add To Role {CurrentUserRole}");
        }
    }

    private async Task SaveUser(bool RefreshUsers)
    {
        try
        {
            #region 已存在的用户
            if (objUser.Id != "")
            {
                // 获取用户
                var user = await UserManager.FindByIdAsync(objUser.Id);
                if (user == null)
                {
                    return;
                }

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
                new ApplicationUser
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

    private async Task EditUser(ApplicationUser _IdentityUser)
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
