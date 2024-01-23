// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using IdentityModel.Client;
using IdentityModel.OidcClient;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ConsoleOIDC;

public class Program
{
    static string authority = "https://localhost:5001/";
    //static string authority = "https://ids2.app1.es/"; //真实环境
    static string api = $"{authority}WeatherForecast";
    static string clientId = "Blazor5002";

    static OidcClient? _oidcClient;
    static HttpClient _apiClient = new HttpClient { BaseAddress = new Uri(api) };

    public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

    public static async Task MainAsync()
    {
        Console.WriteLine("+-----------------------+");
        Console.WriteLine("|  Sign in with OIDC    |");
        Console.WriteLine("+-----------------------+");
        Console.WriteLine("");

        await Login();
    }

    private static async Task Login()
    {
        // 使用环回地址上的可用端口创建重定向 URI。
        // 要求 OP 允许 127.0.0.1 上的随机端口 - 否则设置静态端口

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

        var serilog = new LoggerConfiguration()
            .MinimumLevel.Error()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
            .CreateLogger();

        options.LoggerFactory.AddSerilog(serilog);

        _oidcClient = new OidcClient(options);
        var result = await _oidcClient.LoginAsync(new LoginRequest());

        ShowResult(result);
        await CallApi(result.AccessToken);
        await NextSteps(result);
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

    private static async Task NextSteps(LoginResult result)
    {
        var currentAccessToken = result.AccessToken;
        var currentRefreshToken = result.RefreshToken;

        var menu = "  x...exit  c...call api   ";
        if (currentRefreshToken != null) menu += "r...refresh token   ";

        while (true)
        {
            Console.WriteLine("\n\n");

            Console.Write(menu);
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.X) return;
            if (key.Key == ConsoleKey.C) await CallApi(currentAccessToken);
            if (key.Key == ConsoleKey.R)
            {
                var refreshResult = await _oidcClient.RefreshTokenAsync(currentRefreshToken);
                if (refreshResult.IsError)
                {
                    Console.WriteLine($"Error: {refreshResult.Error}");
                }
                else
                {
                    currentRefreshToken = refreshResult.RefreshToken;
                    currentAccessToken = refreshResult.AccessToken;

                    Console.WriteLine("\n\n");
                    Console.WriteLine($"access token:   {refreshResult.AccessToken}");
                    Console.WriteLine($"refresh token:  {refreshResult?.RefreshToken ?? "none"}");
                }
            }
        }
    }

    private static async Task CallApi(string currentAccessToken)
    {
        try
        {
            _apiClient.SetBearerToken(currentAccessToken);
            var response = await _apiClient.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {str}");
                var json = JArray.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine("\n\n");
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");

        }
    }
}
