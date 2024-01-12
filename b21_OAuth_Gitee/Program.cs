// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b21_OAuth_Gitee;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//google

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = "https://accounts.google.com/";
    options.ProviderOptions.ClientId = "*******.apps.googleusercontent.com";
    options.ProviderOptions.ResponseType = "id_token token";
    options.ProviderOptions.RedirectUri = "https://localhost:5001/authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = "/";
    options.UserOptions.AuthenticationType = "google";
});

//gitee 没有提供 OIDC，只能用 OAuth2


await builder.Build().RunAsync();
