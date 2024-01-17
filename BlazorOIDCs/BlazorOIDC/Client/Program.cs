// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BlazorOIDC.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var useServerAPI = false;

//使用真实的演示站点的API
var useServerAPIDemo = true;

//只是WASM客户端,不使用自托管服务器端API
var isClientOnly = true;

/* 
 * wasm asp.net core 自托管oidc授权: useServerAPI = false, useServerAPIDemo = false, isClientOnly = false
 * wasm 纯客户端 oidc授权: useServerAPI = false, useServerAPIDemo = false, isClientOnly = true
 * wasm 纯客户端 oidc 真实的演示站点API授权: useServerAPI = false, useServerAPIDemo = true, isClientOnly = true
 */


var serverAPIclientID = !useServerAPIDemo?"BlazorOIDC.ServerAPI":"BlazorWasmIdentity.ServerAPI";
var oidcClientID = !useServerAPIDemo? "BlazorOIDC.Client" : "BlazorWasmIdentity.Localhost";

//服务器应用的 API 授权终结点 URL
var apiEndPoint = string.Empty;
var callBackURL = string.Empty;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

apiEndPoint = !isClientOnly? builder.HostEnvironment.BaseAddress:"https://ids2.app1.es/";
callBackURL = apiEndPoint;

if (useServerAPI)
{
    //如果要将 Blazor WebAssembly 应用配置为使用不属于托管 Blazor 解决方案的现有 Identity Server 实例，请将 HttpClient基地址注册从IWebAssemblyHostEnvironment.BaseAddress ( builder.HostEnvironment.BaseAddress) 更改为服务器应用的 API 授权终结点 URL。

    builder.Services.AddHttpClient(serverAPIclientID, client => client.BaseAddress = new Uri(apiEndPoint))
        .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

    // 在向服务器项目发出请求时提供包含访问令牌的 Http 客户端实例
    builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(serverAPIclientID));
    builder.Services.AddApiAuthorization();

}
else
{


 #if DEBUG

    if (isClientOnly)
    {
        callBackURL = apiEndPoint;
        apiEndPoint = "https://ids2.app1.es/";
    }

#else
    apiEndPoint = "https://ids2.app1.es/";
    callBackURL = apiEndPoint;
    oidcClientID = "BlazorWasmIdentity.Client";
#endif

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiEndPoint) });

    //完整的配置
    builder.Services.AddOidcAuthentication(options =>
    {
        options.ProviderOptions.Authority = apiEndPoint;
        options.ProviderOptions.ClientId = oidcClientID;
        options.ProviderOptions.ResponseType = "code";
        options.ProviderOptions.RedirectUri = $"{callBackURL}authentication/login-callback";
        options.ProviderOptions.PostLogoutRedirectUri = $"{callBackURL}authentication/logout-callback";
        options.ProviderOptions.DefaultScopes.Add("openid");
        options.ProviderOptions.DefaultScopes.Add("profile");
    });

    //用第三方登录,只能验证是否登录, 不能验证用户是否有权限. 调用接口时,还是需要验证用户是否有权限


    //IDS演示站点登录 https://demo.duendesoftware.com/
    //builder.Services.AddOidcAuthentication(options =>
    //{
    //    options.ProviderOptions.Authority = "https://demo.duendesoftware.com/";
    //    options.ProviderOptions.ClientId = "interactive.confidential";
    //    options.ProviderOptions.ResponseType = "code";
    //    options.ProviderOptions.RedirectUri = $"{callBackURL}authentication/login-callback";
    //    options.ProviderOptions.PostLogoutRedirectUri = $"{callBackURL}authentication/logout-callback"; 

    //});
}

await builder.Build().RunAsync();
