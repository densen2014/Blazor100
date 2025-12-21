using Microsoft.Maui.Controls;
using SignInMauiApp.Models;
using FreeSql;

namespace SignInMauiApp;

public partial class SignInHistoryPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private readonly User _user;
    private readonly Tenant _tenant;

    public SignInHistoryPage(User user, Tenant tenant)
    {
        InitializeComponent();
        _fsql = IPlatformApplication.Current?.Services.GetService<IFreeSql>();
        _user = user;
        _tenant = tenant;
        TitleLabel.Text = $"{_user.Username} 的签到历史（租户：{_tenant.Name}）";
        LoadHistory();
    }

    private void LoadHistory()
    {
        var records = _fsql!.Select<SignInRecord>()
            .Where(r => r.UserId == _user.Id && r.TenantId == _tenant.Id)
            .OrderByDescending(r => r.SignInTime)
            .ToList();
        HistoryCollectionView.ItemsSource = records;
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
    }
}
