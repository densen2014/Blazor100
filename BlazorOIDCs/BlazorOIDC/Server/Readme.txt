目录:

1. [OpenID 与 OAuth2 基础知识](https://www.cnblogs.com/densen2014/p/17959842)
2. [Blazor wasm Google 登录](https://www.cnblogs.com/densen2014/p/17959857)
3. [Blazor wasm Gitee 码云登录](https://www.cnblogs.com/densen2014/p/17959844)
4. [Blazor SSR/WASM IDS/OIDC 单点登录授权实例讲解1](https://www.cnblogs.com/densen2014/p/17959982)
5. [Blazor SSR/WASM IDS/OIDC 单点登录授权实例讲解2](https://www.cnblogs.com/densen2014/p/17959984)
6. [Blazor SSR/WASM IDS/OIDC 单点登录授权实例讲解3](https://www.cnblogs.com/densen2014/p/17959986)

### 源码

[BlazorOIDC/Server](https://github.com/densen2014/Blazor100/tree/master/BlazorOIDCs/BlazorOIDC/Server)

### 1. 建立 BlazorOIDC 工程

新建wasm工程 BlazorOIDC

![](https://img2024.cnblogs.com/blog/1980213/202401/1980213-20240117080830445-1210021273.png)


* 框架: 7.0
* 身份验证类型: 个人账户 
* ASP.NET Core 托管

### 2. 添加自定义身份实体类,扩展IDS字段

**BlazorOIDC.Server项目**

编辑 Models/WebAppIdentityUser.cs 文件

```
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlazorOIDC.Server.Models;

public class ApplicationUser : IdentityUser
{ 
    /// <summary>
    /// Full name
    /// </summary>
    [Display(Name = "全名")]
    [PersonalData]
    public string? Name { get; set; }

    /// <summary>
    /// Birth Date
    /// </summary>
    [Display(Name = "生日")]
    [PersonalData]
    public DateTime? DOB { get; set; }

    [Display(Name = "识别码")]
    public string? UUID { get; set; }

    [Display(Name = "外联")]
    public string? provider { get; set; }

    [Display(Name = "税号")]
    [PersonalData]
    public string? TaxNumber { get; set; }

    [Display(Name = "街道地址")]
    [PersonalData]
    public string? Street { get; set; }

    [Display(Name = "邮编")]
    [PersonalData]
    public string? Zip { get; set; }

    [Display(Name = "县")]
    [PersonalData]
    public string? County { get; set; }

    [Display(Name = "城市")]
    [PersonalData]
    public string? City { get; set; }

    [Display(Name = "省份")]
    [PersonalData]
    public string? Province { get; set; }

    [Display(Name = "国家")]
    [PersonalData]
    public string? Country { get; set; }

    [Display(Name = "类型")]
    [PersonalData]
    public string? UserRole { get; set; }
}

```

### 3. 添加自定义声明

**BlazorOIDC.Server项目**


新建 Data/ApplicationUserClaimsPrincipalFactory.cs 文件

``` 
using BlazorOIDC.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Densen.Models.ids;


public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> role,IOptions<IdentityOptions> optionsAccessor) : base(userManager, role, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        ClaimsIdentity claims = await base.GenerateClaimsAsync(user);
        var roles = await UserManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.AddClaim(new Claim("roleVIP", role));
        }
        return claims;
    }

}
``` 

### 4. 配置文件

**BlazorOIDC.Server项目**


引用 `Microsoft.EntityFrameworkCore.Sqlite` 包, 示例使用sqlite数据库演示
引用第三方登录包

```
Microsoft.AspNetCore.Authentication.Facebook
Microsoft.AspNetCore.Authentication.Google
Microsoft.AspNetCore.Authentication.MicrosoftAccount
Microsoft.AspNetCore.Authentication.Twitter
AspNet.Security.OAuth.GitHub
```

编辑配置文件 `appsettings.json`, 添加连接字符串和第三方登录ClientId/ClientSecret等配置

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-BlazorOIDC.Server-e292861d-0c29-45ea-84b1-b4558d5aa35d;Trusted_Connection=True;MultipleActiveResultSets=true",
    "IdsSQliteConnection": "Data Source=../ids_api.db;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "IdentityServer": {
    "Clients": {
      "BlazorOIDC.Client": {
        "Profile": "IdentityServerSPA"
      }
    }
  },
  "AllowedHosts": "*",
  "Authentication": {
    "Google": {
      "Instance": "https://accounts.google.com/o/oauth2/v2/auth",
      "ClientId": "ClientId",
      "ClientSecret": "ClientSecret",
      "CallbackPath": "/signin-google"
    },
    "Facebook": {
      "AppId": "AppId",
      "AppSecret": "AppSecret"
    },
    "Microsoft": {
      "ClientId": "ClientId",
      "ClientSecret": "ClientSecret"
    },
    "Twitter": {
      "ConsumerAPIKey": "ConsumerAPIKey",
      "ConsumerSecret": "ConsumerSecret"
    },
    "Github": {
      "ClientID": "ClientID",
      "ClientSecret": "ClientSecret"
    },
    "WeChat": {
      "AppId": "AppId",
      "AppSecret": "AppSecret"
    },
    "QQ": {
      "AppId": "AppId",
      "AppKey": "AppKey"
    }
  }
}
```

### 5. 配置IDS身份验证服务

**BlazorOIDC.Server项目**

编辑 Program.cs 文件
  
```
using BlazorOIDC.Server.Data;
using BlazorOIDC.Server.Models;
using Densen.Identity.Areas.Identity;
using Densen.Models.ids;
using Duende.IdentityServer;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

//EF Sqlite 配置
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("IdsSQliteConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//附加自定义用户声明到用户主体
builder.Services.AddScoped<ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddDefaultIdentity<ApplicationUser>(o =>
{   // Password settings.
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequiredLength = 1;
    o.Password.RequiredUniqueChars = 1;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddIdentityServer(options =>
{
    options.LicenseKey = builder.Configuration["IdentityServerLicenseKey"];

    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.IdentityResources["openid"].UserClaims.Add("roleVIP");

        // Client localhost
        var url2 = "localhost";
        var spaClient2 = ClientBuilder
            .SPA("BlazorWasmIdentity.Localhost")
            .WithRedirectUri($"https://{url2}:5001/authentication/login-callback")
            .WithLogoutRedirectUri($"https://{url2}:5001/authentication/logout-callback")
            .WithScopes("openid Profile")
            .Build();
        spaClient2.AllowOfflineAccess = true;

        spaClient2.AllowedCorsOrigins = new[]
        {
            $"https://{url2}:5001"
        };

        options.Clients.Add(spaClient2);
    });

 
builder.Services.AddAuthentication();

var autbuilder = new AuthenticationBuilder(builder.Services);
autbuilder.AddGoogle(o =>
{
    o.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    o.ClaimActions.MapJsonKey("urn:google:profile", "link");
    o.ClaimActions.MapJsonKey("urn:google:image", "picture");
});
//autbuilder.AddFacebook(o =>
//{
//    o.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
//    o.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
//});
//autbuilder.AddTwitter(o =>
//{
//    o.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerAPIKey"] ?? "";
//    o.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"] ?? "";
//    o.RetrieveUserDetails = true;
//});
autbuilder.AddGitHub(o =>
{
    o.ClientId = builder.Configuration["Authentication:Github:ClientID"] ?? "";
    o.ClientSecret = builder.Configuration["Authentication:Github:ClientSecret"] ?? "";
});
//autbuilder.AddMicrosoftAccount(o =>
//{
//    o.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? "";
//    o.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? "";
//});
//if (WeChat) autbuilder.AddWeChat(o =>
//{
//    o.AppId = Configuration["Authentication:WeChat:AppId"];
//    o.AppSecret = Configuration["Authentication:WeChat:AppSecret"];
//    o.UseCachedStateDataFormat = true;
//})
//autbuilder.AddQQ(o =>
//{
//    o.AppId = builder.Configuration["Authentication:QQ:AppId"] ?? "";
//    o.AppKey = builder.Configuration["Authentication:QQ:AppKey"] ?? "";
//});
autbuilder.AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
{
    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
    options.SaveTokens = true;

    options.Authority = "https://demo.duendesoftware.com";
    options.ClientId = "interactive.confidential";
    options.ClientSecret = "secret";
    options.ResponseType = "code";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseIdentityServer();
app.UseAuthorization();


app.MapBlazorHub();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
```

### 6. 运行工程

因为篇幅的关系,具体数据库改为sqlite生成脚本步骤参考[以前文章](https://www.cnblogs.com/densen2014/p/17083934.html)或者直接拉源码测试


* 点击注册按钮
* 用户名 test@test.com
* 密码 1qaz2wsx

![](https://img2024.cnblogs.com/blog/1980213/202401/1980213-20240117083051216-484046550.png)

* 点击 Apply Migrations 按钮

![](https://img2024.cnblogs.com/blog/1980213/202401/1980213-20240117083224935-1183310317.png)

* 刷新页面

![](https://img2024.cnblogs.com/blog/1980213/202401/1980213-20240117084324118-1140920227.png)

* 已经可以成功登录
