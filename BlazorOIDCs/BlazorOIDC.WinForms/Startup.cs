// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BlazorOIDC.WinForms.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace BlazorOIDC.WinForms;
public static class Startup
{
    public static IServiceProvider? Services { get; private set; }

    public static void Init()
    {
        var host = Host.CreateDefaultBuilder()
                       .ConfigureServices(WireupServices)
                       .Build();
        Services = host.Services;
    }

    private static void WireupServices(IServiceCollection services)
    {
        services.AddWindowsFormsBlazorWebView();
        services.AddSingleton<WeatherForecastService>();

        services.AddAuthorizationCore();
        services.TryAddScoped<AuthenticationStateProvider, ExternalAuthStateProvider>();
        services.AddSingleton<AuthenticatedUser>();
 
  

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
    }
}
