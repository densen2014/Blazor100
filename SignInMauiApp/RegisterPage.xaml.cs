using Microsoft.Maui.Controls;
using SignInMauiApp.Models;
using FreeSql;

namespace SignInMauiApp;

public partial class RegisterPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private List<Tenant> _tenants = new();

    public RegisterPage()
    {
        InitializeComponent();
        _fsql = IPlatformApplication.Current?.Services.GetService<IFreeSql>();
        LoadTenants();
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

    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        var newTenant = NewTenantEntry.Text?.Trim();
        int tenantId = -1;
        if (!string.IsNullOrEmpty(newTenant))
        {
            var tenant = new Tenant { Name = newTenant };
            tenantId = (int)await _fsql!.Insert(tenant).ExecuteIdentityAsync();
        }
        else if (TenantPicker.SelectedIndex >= 0)
        {
            tenantId = _tenants[TenantPicker.SelectedIndex].Id;
        }
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || tenantId < 0)
        {
            ErrorLabel.Text = "请填写完整信息";
            ErrorLabel.IsVisible = true;
            return;
        }
        if (_fsql!.Select<User>().Any(u => u.Username == username && u.TenantId == tenantId))
        {
            ErrorLabel.Text = "该用户已存在";
            ErrorLabel.IsVisible = true;
            return;
        }
        var user = new User { Username = username, Password = password, TenantId = tenantId };
        await _fsql!.Insert(user).ExecuteAffrowsAsync();
        await DisplayAlertAsync("注册成功", "请返回登录", "确定");
        await Navigation.PopAsync();
    }
}
