// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using IdentityModel.OidcClient;
using System.Text;
using WinFormsWebView2;

namespace WinFormsOIDC;

public partial class Form1 : Form
{
    static string authority = "https://localhost:5001/";
    //static string authority = "https://ids2.app1.es/"; //真实环境
    static string api = $"{authority}WeatherForecast";
    static string clientId = "Blazor5002";

    OidcClient _oidcClient;

    public Form1()
    {
        InitializeComponent();
        string redirectUri = string.Format($"http://localhost/authentication/login-callback");
        string redirectLogoutUri = string.Format($"http://localhost/authentication/logout-callback");

        var options = new OidcClientOptions
        {
            Authority = authority,
            ClientId = clientId,
            RedirectUri = redirectUri,
            PostLogoutRedirectUri = redirectLogoutUri,
            Scope = "BlazorWasmIdentity.ServerAPI openid profile",
            Browser = new WinFormsWebView()
        };

        _oidcClient = new OidcClient(options);

        Login();
    }

    private async void Login()
    {
        LoginResult loginResult;

        try
        {
            loginResult = await _oidcClient.LoginAsync();
        }
        catch (Exception exception)
        {
            Output.Text = $"Unexpected Error: {exception.Message}";
            return;
        }


        if (loginResult.IsError)
        {
            MessageBox.Show(this, loginResult.Error, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            var sb = new StringBuilder(128);
            foreach (var claim in loginResult.User.Claims)
            {
                sb.AppendLine($"{claim.Type}: {claim.Value}");
            }

            if (!string.IsNullOrWhiteSpace(loginResult.RefreshToken))
            {
                sb.AppendLine();
                sb.AppendLine($"refresh token: {loginResult.RefreshToken}");
            }

            if (!string.IsNullOrWhiteSpace(loginResult.IdentityToken))
            {
                sb.AppendLine();
                sb.AppendLine($"identity token: {loginResult.IdentityToken}");
            }

            if (!string.IsNullOrWhiteSpace(loginResult.AccessToken))
            {
                sb.AppendLine();
                sb.AppendLine($"access token: {loginResult.AccessToken}");
            }

            Output.Text = sb.ToString();
        }
    }
}
