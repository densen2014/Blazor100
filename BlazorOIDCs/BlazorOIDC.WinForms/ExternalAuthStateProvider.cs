// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorOIDC.WinForms;

public class ExternalAuthStateProvider : AuthenticationStateProvider
{
    private readonly Task<AuthenticationState> authenticationState;

    public ExternalAuthStateProvider(AuthenticatedUser user) =>
        authenticationState = Task.FromResult(new AuthenticationState(user.Principal));

    private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(currentUser));

    public Task<AuthenticationState> LogInAsync()
    {
        var loginTask = LogInAsyncCore();
        NotifyAuthenticationStateChanged(loginTask);

        return loginTask;

        async Task<AuthenticationState> LogInAsyncCore()
        {
            var user = await LoginWithExternalProviderAsync();
            currentUser = user;

            return new AuthenticationState(currentUser);
        }
    }

    private async Task<ClaimsPrincipal> LoginWithExternalProviderAsync()
    {
        /*
            提供 Open ID/MSAL 代码以对用户进行身份验证。查看您的身份
            提供商的文档以获取详细信息。

            根据新的声明身份返回新的声明主体。
        */

        string authority = "https://localhost:5001/";
        //string authority = "https://ids2.app1.es/"; //真实环境
        string api = $"{authority}WeatherForecast";
        string clientId = "Blazor5002";

        OidcClient? _oidcClient;
        HttpClient _apiClient = new HttpClient { BaseAddress = new Uri(api) };

        var browser = new SystemBrowser(5002);
        var redirectUri = string.Format($"http://localhost:{browser.Port}/authentication/login-callback");
        var redirectLogoutUri = string.Format($"http://localhost:{browser.Port}/authentication/logout-callback");

        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = clientId,
            RedirectUri = redirectUri,
            PostLogoutRedirectUri = redirectLogoutUri,
            Scope = "BlazorWasmIdentity.ServerAPI openid profile",
            //Scope = "Blazor7.ServerAPI openid profile",
            Browser = browser,
            Policy = new Policy { RequireIdentityTokenSignature = false }

        };

        _oidcClient = new OidcClient(options);
        var result = await _oidcClient.LoginAsync(new LoginRequest());
        ShowResult(result);

        var authenticatedUser = result.User;

        return authenticatedUser;
    }

    private static void ShowResult(LoginResult result, bool showToken = false)
    {
        if (result.IsError)
        {
            Console.WriteLine("\n\nError:\n{0}", result.Error);
            return;
        }

        Console.WriteLine("\n\nClaims:");
        foreach (var claim in result.User.Claims)
        {
            Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
        }

        if (showToken)
        {
            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"access token:   {result.AccessToken}");
            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
        }
    }

    public Task Logout()
    {
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentUser)));
        return Task.CompletedTask;
    }
}

public class AuthenticatedUser
{
    public ClaimsPrincipal Principal { get; set; } = new();
}

