// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace BlazorOIDC.WinForms;
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        var blazor = new BlazorWebView()
        {
            Dock = DockStyle.Fill,
            HostPage = "wwwroot/index.html",
            Services = Startup.Services!,
            StartPath = "/Login"
        };
        blazor.RootComponents.Add<Main>("#app");
        Controls.Add(blazor);

        //var authenticatedUser = Startup.Services!.GetRequiredService<AuthenticatedUser>();

        ///*
        //Provide OpenID/MSAL code to authenticate the user. See your identity provider's 
        //documentation for details.

        //The user is represented by a new ClaimsPrincipal based on a new ClaimsIdentity.
        //*/
        //var user = new ClaimsPrincipal(new ClaimsIdentity());

        //authenticatedUser.Principal = user;
    }
}
