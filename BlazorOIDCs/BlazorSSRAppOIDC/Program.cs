// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BlazorSSRAppOIDC.Components;
using OidcClientShared;

var builder = WebApplication.CreateBuilder(args);

//在具有 Blazor Web 应用程序模板的 .NET 8 中，需要将其更改为, 由于该Pages文件夹已移至该Components文件夹中，因此您需要指定新位置的根目录，或将该Pages文件夹移回项目的根级别
builder.Services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpClient();
builder.Services.AddDensenExtensions();
builder.Services.ConfigureJsonLocalizationOptions(op =>
{
    // 忽略文化信息丢失日志
    op.IgnoreLocalizerMissing = true;

});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", OidcProfile.OidcDIY); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
//但出于某种原因，这还不够。在 Blazor Web 应用程序模板中，您明确需要调用
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
