using Microsoft.Maui.Controls;
using SignInMauiApp.Models;
using FreeSql;

namespace SignInMauiApp;

public partial class TenantManagementPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private List<Tenant> _tenants = new();

    public TenantManagementPage()
    {
        InitializeComponent();
        _fsql = IPlatformApplication.Current?.Services.GetService<IFreeSql>();
        LoadTenants();
    }

    private void LoadTenants()
    {
        _tenants = _fsql!.Select<Tenant>().ToList();
        TenantCollectionView.ItemsSource = _tenants;
    }

    private async void OnAddTenantClicked(object sender, EventArgs e)
    {
        var name = NewTenantEntry.Text?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        if (_fsql!.Select<Tenant>().Any(t => t.Name == name))
        {
            await DisplayAlertAsync("提示", "公司已存在", "确定");
            return;
        }
        await _fsql!.Insert(new Tenant { Name = name }).ExecuteAffrowsAsync();
        NewTenantEntry.Text = string.Empty;
        LoadTenants();
    }

    private async void OnEditTenantClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Tenant tenant)
        {
            string result = await DisplayPromptAsync("编辑公司", "请输入新名称", initialValue: tenant.Name);
            if (!string.IsNullOrEmpty(result) && result != tenant.Name)
            {
                tenant.Name = result;
                await _fsql!.Update<Tenant>().SetSource(tenant).ExecuteAffrowsAsync();
                LoadTenants();
            }
        }
    }

    private async void OnDeleteTenantClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Tenant tenant)
        {
            if (await DisplayAlertAsync("确认", $"确定删除公司：{tenant.Name}？", "删除", "取消"))
            {
                await _fsql!.Delete<Tenant>().Where(t => t.Id == tenant.Id).ExecuteAffrowsAsync();
                LoadTenants();
            }
        }
    }

    private void OnTenantSelected(object sender, SelectionChangedEventArgs e)
    {
        // 可扩展：选中公司后显示详情或关联用户
    }
}
