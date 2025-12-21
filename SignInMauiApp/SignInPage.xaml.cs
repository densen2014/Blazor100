using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using SignInMauiApp.Models;
using FreeSql;
using System.Diagnostics.CodeAnalysis;

namespace SignInMauiApp;

public partial class SignInPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private readonly User _user;
    private readonly Tenant _tenant;

    public SignInPage(User user, Tenant tenant)
    {
        InitializeComponent();
        _fsql = IPlatformApplication.Current?.Services.GetService<IFreeSql>();
        _user = user;
        _tenant = tenant;
        WelcomeLabel.Text = $"欢迎 {_user.Username}，公司：{_tenant.Name}";
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        var record = new SignInRecord
        {
            UserId = _user.Id,
            TenantId = _tenant.Id,
            SignInTime = DateTime.Now
        };
        await _fsql!.Insert(record).ExecuteAffrowsAsync();
        SignInResultLabel.Text = $"签到成功：{record.SignInTime:yyyy-MM-dd HH:mm:ss}";
        SignInResultLabel.IsVisible = true;
        // 跳转到签到历史页面
        await Navigation.PushAsync(new SignInHistoryPage(_user, _tenant));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 添加退出登录按钮
        if (ToolbarItems.All(t => t.Text != "退出登录"))
        {
            ToolbarItems.Add(new ToolbarItem("退出登录", null, async () => {
                await Navigation.PopToRootAsync();
            }));
        }
        // 添加租户管理按钮
        if (ToolbarItems.All(t => t.Text != "公司管理"))
        {
            ToolbarItems.Add(new ToolbarItem("公司管理", null, async () => {
                await Navigation.PushAsync(new TenantManagementPage());
            }));
        }
        // 添加签到报表按钮（仅管理员可见，这里假设用户名为admin为管理员）
        if (_user.Username == "admin" && ToolbarItems.All(t => t.Text != "签到报表"))
        {
            ToolbarItems.Add(new ToolbarItem("签到报表", null, async () => {
                await Navigation.PushAsync(new SignInReportPage());
            }));
        }
    }
}
