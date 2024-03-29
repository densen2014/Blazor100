﻿// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BlazorOIDCAuto;
using BlazorOIDCAuto.Components;
using BlazorOIDCAuto.Components.Account;
using BlazorOIDCAuto.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

//EF Sqlite 配置
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("IdsSQliteConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//附加自定义用户声明到用户主体
builder.Services.AddScoped<ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

//builder.Services.AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddAuthentication();

//添加第三方登录
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
//autbuilder.AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
//{
//    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
//    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
//    options.SaveTokens = true;

//    options.Authority = "https://demo.duendesoftware.com";
//    options.ClientId = "interactive.confidential";
//    options.ClientSecret = "secret";
//    options.ResponseType = "code";

//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        NameClaimType = "name",
//        RoleClaimType = "role"
//    };
//});

//builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<WebAppIdentityUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseRouting();

app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//app.UseIdentityServer();
//app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorOIDCAuto.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
