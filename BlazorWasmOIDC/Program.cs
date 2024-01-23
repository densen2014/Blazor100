// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BlazorWasmOIDC;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var authority = "https://localhost:5001/";
var clientId = "Blazor5002";
var url2 = builder.HostEnvironment.BaseAddress;

//完整的配置
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = authority;
    options.ProviderOptions.ClientId = clientId;
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.RedirectUri = $"{url2}authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = $"{url2}authentication/logout-callback";
    options.ProviderOptions.DefaultScopes.Add("BlazorWasmIdentity.ServerAPI");
    options.UserOptions.RoleClaim = "role";
});

await builder.Build().RunAsync();
