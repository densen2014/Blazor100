using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using FreeSql;
using SignInMauiApp.Models;

namespace SignInMauiApp;

public partial class OnboardingPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private Entry _tenantEntry;
    private Entry _adminUserEntry;
    private Entry _adminPassEntry;
    private Entry _firstUserEntry;
    private Entry _firstUserPassEntry;
    private Button _submitButton;

    public OnboardingPage(IFreeSql? fsql)
    {
        _fsql = fsql;
        Title = "初次设置";
        _tenantEntry = new Entry { Placeholder = "请输入公司名称,默认为[我的公司]" };
        _adminUserEntry = new Entry { Placeholder = "管理员账号,默认为[admin]" };
        _adminPassEntry = new Entry { Placeholder = "管理员密码", IsPassword = true };
        _firstUserEntry = new Entry { Placeholder = "第一个用户名称,默认为[demo]" };
        _firstUserPassEntry = new Entry { Placeholder = "第一个用户密码,默认为[0]", IsPassword = true };
        _submitButton = new Button { Text = "完成设置" };
        _submitButton.Clicked += OnSubmitClicked;
        Content = new VerticalStackLayout
        {
            Padding = 30,
            Children =
            {
                new Label { Text = "欢迎使用，请完成初始设置：", FontSize = 20, HorizontalOptions = LayoutOptions.Center },
                _tenantEntry,
                _adminUserEntry,
                _adminPassEntry,
                _firstUserEntry,
                _firstUserPassEntry,
                _submitButton
            }
        };
    }

    private async void OnSubmitClicked(object? sender, EventArgs e)
    {
        var tenantName = _tenantEntry.Text?.Trim()?? "我的公司";
        var adminUser = _adminUserEntry.Text?.Trim()??"admin";
        var adminPass = _adminPassEntry.Text?.Trim();
        var firstUser = _firstUserEntry.Text?.Trim() ?? "demo";
        var firstUserPass = _firstUserPassEntry.Text?.Trim() ?? "0";
        if (string.IsNullOrEmpty(tenantName) || string.IsNullOrEmpty(adminUser) || string.IsNullOrEmpty(adminPass) || string.IsNullOrEmpty(firstUser))
        {
            await DisplayAlertAsync("提示", "请填写所有信息", "确定");
            return;
        }
        // 创建租户
        var tenant = new Tenant { Name = tenantName };
        _fsql?.Insert(tenant).ExecuteAffrows();
        // 创建管理员
        var admin = new User { Username = adminUser, Password = adminPass, IsAdmin = true, TenantId = tenant.Id };
        _fsql?.Insert(admin).ExecuteAffrows();
        // 创建第一个普通用户
        var user = new User { Username = firstUser, Password = firstUserPass, IsAdmin = false, TenantId = tenant.Id };
        _fsql?.Insert(user).ExecuteAffrows();
        Preferences.Set("OnboardingDone", true);
        await DisplayAlertAsync("完成", "初始化完成！", "进入系统");
        await Navigation.PopModalAsync();
    }
}
