// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

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
