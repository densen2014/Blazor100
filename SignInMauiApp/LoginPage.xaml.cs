using Microsoft.Maui.Controls;
using SignInMauiApp.Models;
using FreeSql;
using Microsoft.Maui.Storage;

namespace SignInMauiApp;

public partial class LoginPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private List<Tenant> _tenants = new();

    public LoginPage()
    {
        InitializeComponent();
        _fsql = IPlatformApplication.Current?.Services.GetService<IFreeSql>();
        // 新增：检查是否需要显示引导页
        CheckAndShowOnboardingAsync();
        LoadTenants();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadTenants(); // 注册后刷新租户列表
    }

    private async void CheckAndShowOnboardingAsync()
    {
        // 检查本地存储是否已完成引导
        var onboardingDone = Preferences.Get("OnboardingDone", false);
        if (!onboardingDone)
        {
            await Navigation.PushModalAsync(new OnboardingPage(_fsql));
        }
    }

    private void LoadTenants()
    {
        _tenants = _fsql!.Select<Tenant>().ToList();
        TenantPicker.ItemsSource = _tenants.Select(t => t.Name).ToList();
        int lastTenantId = Preferences.Get("LastTenantId", -1);
        int idx = 0;
        if (_tenants.Count > 0)
        {
            if (lastTenantId > 0)
            {
                idx = _tenants.FindIndex(t => t.Id == lastTenantId);
                if (idx < 0) idx = 0;
            }
            TenantPicker.SelectedIndex = idx;
            //TenantPicker.IsEnabled = false;
        }
        UsernameEntry.Text = Preferences.Get("LastUsername", "");
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        var tenantIdx = TenantPicker.SelectedIndex;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || tenantIdx < 0)
        {
            ErrorLabel.Text = "请填写完整信息";
            ErrorLabel.IsVisible = true;
            return;
        }
        var tenantId = _tenants[tenantIdx].Id;
        var user = _fsql!.Select<User>().Where(u => u.Username == username && u.Password == password && u.TenantId == tenantId).First();
        if (user == null)
        {
            ErrorLabel.Text = "用户名或密码错误";
            ErrorLabel.IsVisible = true;
            return;
        }
        // 记住租户和用户名
        Preferences.Set("LastTenantId", tenantId);
        Preferences.Set("LastUsername", username);
        // 登录成功，跳转到签到页面
        await Navigation.PushAsync(new SignInPage(user, _tenants[tenantIdx]));
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}
