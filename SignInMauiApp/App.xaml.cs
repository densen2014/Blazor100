// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.Extensions.DependencyInjection;

namespace SignInMauiApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        InitializeData();
    }

    private void InitializeData()
    {
        var fsql = MauiProgram.CreateMauiApp().Services.GetService<IFreeSql>();
        if (fsql == null) return;
        // 初始化租户
        if (!fsql.Select<Models.Tenant>().Any())
        {
            var tenant = new Models.Tenant { Name = "默认租户" };
            fsql.Insert(tenant).ExecuteAffrows();
            // 初始化用户
            fsql.Insert(new Models.User { Username = "admin", Password = "123456", TenantId = tenant.Id }).ExecuteAffrows();
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
